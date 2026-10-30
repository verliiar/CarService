namespace CarService.Domain;

public sealed record CarSnapshot(
    string Brand,
    string Model,
    decimal Price,
    List<DetailSnapshot> RequiredDetails,
    string Body,
    EngineSnapshot Engine,
    string Color,
    bool IsAvailable)
{
    public static CarSnapshot From(Car car, IEnumerable<Component>? configuration = null, decimal? price = null) =>
        new(car.Brand, car.Model, price ?? car.Price,
            (configuration ?? car.RequiredDetails).Select(component => new DetailSnapshot(component.Category.ToString(), component.Name)).ToList(),
            car.Body.ToString(), new EngineSnapshot(car.Fuel.ToString(), car.EnginePower, car.EngineDisplacement), car.Color, car.IsAvailable);
}
