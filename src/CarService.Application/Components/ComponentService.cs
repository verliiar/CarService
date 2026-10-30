using CarService.Domain;

namespace CarService.Application;

public sealed class ComponentService(IRepository<Component> components, CarCatalog cars, UserService users, IUnitOfWork unit)
{
    public static ComponentCategory Category(string route) => route switch
    {
        "interior" => ComponentCategory.Interior,
        "steering_wheel" => ComponentCategory.SteeringWheel,
        "transmission" => ComponentCategory.Transmission,
        "wheel" => ComponentCategory.Wheel,
        _ => throw new BusinessException(FailureKind.NotFound, "Component category not found.")
    };

    public static object Map(Component c) => c switch
    {
        Interior x => new InteriorDto(x.Category.ToString(), x.Id, x.Name, x.AdditionalCost, x.Material),
        SteeringWheel x => new SteeringWheelDto(x.Category.ToString(), x.Id, x.Name, x.AdditionalCost, x.Size, x.Material, x.Shape),
        Transmission x => new TransmissionDto(x.Category.ToString(), x.Id, x.Name, x.AdditionalCost, x.Gearbox.ToString(), x.WheelDrive.ToString(), x.EnergyTransfer.ToString()),
        Wheel x => new WheelDto(x.Category.ToString(), x.Id, x.Name, x.AdditionalCost, x.Diameter, x.LoadIndex, x.Seasonality.ToString()),
        _ => throw new BusinessException(FailureKind.Validation, "Invalid component type.")
    };

    public async Task<List<object>> ReadAsync(Guid userId, ComponentCategory category, string? name, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, UserRole.WarehouseAdmin, UserRole.SystemAdmin);
        var list = await components.ListAsync(q => q.Where(c => !c.IsRemoved && c.Category == category && (name == null || c.Name == name)).OrderBy(c => c.Name), ct, false);
        if (name != null && list.Count == 0)
        {
            throw new BusinessException(FailureKind.NotFound, "Component not found.");
        }

        return list.Select(Map).ToList();
    }

    public async Task SaveAsync(Guid userId, ComponentCategory category, DetailRequest request, bool edit, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, UserRole.WarehouseAdmin, UserRole.SystemAdmin);
        var existing = (await components.ListAsync(q => q.Where(c => c.Category == category && c.Name == request.DetailName), ct)).SingleOrDefault();
        if (edit && (existing == null || existing.IsRemoved))
        {
            throw new BusinessException(FailureKind.NotFound, "Component not found.");
        }

        if (!edit && existing is { IsRemoved: false })
        {
            return;
        }

        Component component = existing ?? category switch
        {
            ComponentCategory.Interior => new Interior(),
            ComponentCategory.SteeringWheel => new SteeringWheel(),
            ComponentCategory.Transmission => new Transmission(),
            _ => new Wheel()
        };
        component.Name = Rules.Text(edit ? request.NewDetailName : request.DetailName);
        component.AdditionalCost = new Money(request.AdditionalCost).Value;
        switch (component)
        {
            case Interior x:
                x.Material = Rules.Text(request.Material);
                break;
            case SteeringWheel x:
                if (request.Size <= 0)
                {
                    throw new BusinessException(FailureKind.Validation, "Size must be positive.");
                }

                x.Size = Rules.Measurement(request.Size, true);
                x.Material = Rules.Text(request.Material);
                x.Shape = Rules.Text(request.Shape);
                break;
            case Transmission x:
                x.Gearbox = Rules.Parse<GearboxType>(request.Gearbox);
                x.WheelDrive = Rules.Parse<WheelDriveType>(request.WheelDrive);
                x.EnergyTransfer = Rules.Parse<EnergyTransferType>(request.EnergyTransfer);
                break;
            case Wheel x:
                if (request.Diameter <= 0 || request.LoadIndex <= 0)
                {
                    throw new BusinessException(FailureKind.Validation, "Diameter and load index must be positive.");
                }

                x.Diameter = Rules.Measurement(request.Diameter, true);
                x.LoadIndex = request.LoadIndex;
                x.Seasonality = Rules.Parse<SeasonalityType>(request.Seasonality);
                break;
        }
        var supported = await cars.ResolveCarsAsync(request.SupportedCars, ct);
        await cars.EnsureBaseCompatibilityAsync(component.Id, supported, ct);
        component.SupportedCars = supported;
        component.IsRemoved = false;
        if (existing == null)
        {
            components.Add(component);
        }

        await unit.SaveAsync(ct);
    }

    public async Task DeleteAsync(Guid userId, ComponentCategory category, string name, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, UserRole.SystemAdmin);
        var component = Rules.Found((await components.ListAsync(q => q.Where(c => !c.IsRemoved && c.Category == category && c.Name == name), ct)).SingleOrDefault());
        await cars.RequireUnusedComponentAsync(component.Id, ct);
        component.IsRemoved = true;
        component.SupportedCars.Clear();
        await unit.SaveAsync(ct);
    }
}
