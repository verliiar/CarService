namespace CarService.Domain;

public sealed class Car : Entity
{
    public string Brand { get; set; } = "";

    public string Model { get; set; } = "";
    public decimal Price
    {
        get; set;
    }

    public BodyType Body
    {
        get; set;
    }

    public FuelType Fuel
    {
        get; set;
    }

    public decimal EnginePower
    {
        get; set;
    }

    public decimal EngineDisplacement
    {
        get; set;
    }

    public string Color { get; set; } = "";

    public bool IsAvailable
    {
        get; set;
    }

    public uint Version
    {
        get; set;
    }

    public List<Component> RequiredDetails { get; set; } = [];
}
