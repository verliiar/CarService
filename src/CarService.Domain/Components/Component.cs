namespace CarService.Domain;

public abstract class Component : Entity
{
    public string Name { get; set; } = "";

    public ComponentCategory Category
    {
        get; protected set;
    }

    public decimal AdditionalCost
    {
        get; set;
    }

    public uint Version
    {
        get; set;
    }

    public List<Car> SupportedCars { get; set; } = [];
}
