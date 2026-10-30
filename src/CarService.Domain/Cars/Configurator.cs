namespace CarService.Domain;

public static class Configurator
{
    public static CarSnapshot Configure(Car car, IReadOnlyList<Component> details)
    {
        if (car.IsRemoved || car.RequiredDetails.Count == 0 || car.RequiredDetails.Any(c => c.IsRemoved) ||
            car.RequiredDetails.Select(c => c.Category).Distinct().Count() != car.RequiredDetails.Count)
        {
            throw new BusinessException(FailureKind.Validation, "Car has no valid base configuration.");
        }

        if (details.Count != car.RequiredDetails.Count || details.GroupBy(x => x.Category).Any(g => g.Count() != 1) ||
            !details.Select(x => x.Category).ToHashSet().SetEquals(car.RequiredDetails.Select(x => x.Category)))
        {
            throw new BusinessException(FailureKind.Validation, "Specify exactly one component for every required category.");
        }

        foreach (var detail in details)
        {
            if (detail.IsRemoved || !detail.SupportedCars.Any(c => c.Id == car.Id && !c.IsRemoved))
            {
                throw new BusinessException(FailureKind.Validation, "This component is not supported by this car.");
            }
        }

        var price = new Money(car.Price + details.Where(x => !car.RequiredDetails.Any(b => b.Id == x.Id)).Sum(x => x.AdditionalCost));
        return CarSnapshot.From(car, details, price.Value) with
        {
            IsAvailable = true
        };
    }
}
