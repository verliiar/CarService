using CarService.Domain;

namespace CarService.Application;

public sealed class CarCatalog(IRepository<Car> cars, IRepository<Component> components, UserService users, IUnitOfWork unit)
{
    public async Task<Car> FindAsync(string brand, string model, CancellationToken ct, bool tracking = true) => Rules.Found((await cars.ListAsync(q => q.Where(c => c.Brand == brand && c.Model == model && !c.IsRemoved), ct, tracking)).SingleOrDefault());

    public async Task<List<Component>> ResolveAsync(IReadOnlyList<string> names, CancellationToken ct)
    {
        if (names.Any(string.IsNullOrWhiteSpace) || names.Count != names.Distinct().Count())
        {
            throw new BusinessException(FailureKind.Validation, "Component names must be nonempty and distinct.");
        }

        var found = await components.ListAsync(q => q.Where(c => names.Contains(c.Name) && !c.IsRemoved), ct);
        if (names.Any(n => found.Count(c => c.Name == n) != 1))
        {
            throw new BusinessException(FailureKind.Validation, "A component is missing or its name is ambiguous.");
        }

        return found;
    }

    public async Task<List<Car>> ResolveCarsAsync(IReadOnlyDictionary<string, string> names, CancellationToken ct)
    {
        foreach (var pair in names)
        {
            Rules.Text(pair.Key);
            Rules.Text(pair.Value);
        }
        var brands = names.Keys.ToList();
        var models = names.Values.ToList();
        var found = await cars.ListAsync(q => q.Where(c => brands.Contains(c.Brand) && models.Contains(c.Model) && !c.IsRemoved), ct);
        return names.Select(pair => Rules.Found(found.SingleOrDefault(c => c.Brand == pair.Key && c.Model == pair.Value))).ToList();
    }

    public async Task<List<CarSnapshot>> ListAsync(Guid userId, bool available, CarFilter? filter, string? brand, List<string>? details, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, available ? [UserRole.Customer, UserRole.Manager, UserRole.SystemAdmin] : [UserRole.WarehouseAdmin, UserRole.SystemAdmin]);
        var list = await cars.ListAsync(q =>
        {
            q = q.Where(c => !c.IsRemoved && (!available || c.IsAvailable));
            if (filter != null)
            {
                q = filter.Apply(q);
            }

            if (brand != null)
            {
                q = q.Where(c => c.Brand == brand);
            }

            foreach (var name in details ?? [])
            {
                q = q.Where(c => c.RequiredDetails.Any(d => !d.IsRemoved && d.Name == name));
            }

            return q.OrderBy(c => c.Brand).ThenBy(c => c.Model);
        }, ct, false);
        return list.Select(c => CarSnapshot.From(c)).ToList();
    }

    public async Task SaveAsync(Guid userId, CarRequest request, bool edit, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, UserRole.WarehouseAdmin, UserRole.SystemAdmin);
        var details = await ResolveAsync(request.DetailsName, ct);
        if (details.Count == 0 || details.GroupBy(d => d.Category).Any(g => g.Count() != 1))
        {
            throw new BusinessException(FailureKind.Validation, "Only one component per category is allowed.");
        }

        var existing = (await cars.ListAsync(q => q.Where(c => c.Brand == request.Brand && c.Model == request.Model), ct)).SingleOrDefault();
        if (edit && (existing == null || existing.IsRemoved))
        {
            throw new BusinessException(FailureKind.NotFound, "Car not found.");
        }

        if (!edit && existing is { IsRemoved: false })
        {
            return;
        }

        var car = existing ?? new Car();
        car.Brand = Rules.Text(request.Brand);
        car.Model = Rules.Text(edit ? request.NewModelName : request.Model);
        car.Price = new Money(request.Price).Value;
        car.Body = Rules.Parse<BodyType>(request.Body);
        car.Fuel = Rules.Parse<FuelType>(request.Fuel);
        if (request.EnginePower < 0 || request.EngineDisplacement < 0)
        {
            throw new BusinessException(FailureKind.Validation, "Engine values must be nonnegative.");
        }

        car.EnginePower = Rules.Measurement(request.EnginePower);
        car.EngineDisplacement = Rules.Measurement(request.EngineDisplacement);
        car.Color = Rules.Text(request.Color);
        car.IsAvailable = request.IsAvailable;
        car.IsRemoved = false;
        car.RequiredDetails = details;
        foreach (var detail in details)
        {
            if (detail.SupportedCars.All(c => c.Id != car.Id))
            {
                detail.SupportedCars.Add(car);
            }
        }

        if (existing == null)
        {
            cars.Add(car);
        }

        await unit.SaveAsync(ct);
    }

    public async Task RequireUnusedComponentAsync(Guid componentId, CancellationToken ct)
    {
        if (await cars.AnyAsync(c => !c.IsRemoved && c.RequiredDetails.Any(d => d.Id == componentId), ct))
        {
            throw new BusinessException(FailureKind.Conflict, "The component is required by an active car.");
        }
    }

    public async Task EnsureBaseCompatibilityAsync(Guid componentId, IReadOnlyCollection<Car> supported, CancellationToken ct)
    {
        var ids = supported.Select(c => c.Id).ToList();
        if (await cars.AnyAsync(c => !c.IsRemoved && c.RequiredDetails.Any(d => d.Id == componentId) && !ids.Contains(c.Id), ct))
        {
            throw new BusinessException(FailureKind.Conflict, "A required component must remain compatible with its car.");
        }
    }

    public async Task DeleteAsync(Guid userId, string brand, string model, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, UserRole.SystemAdmin);
        (await FindAsync(brand, model, ct)).IsRemoved = true;
        await unit.SaveAsync(ct);
    }
}
