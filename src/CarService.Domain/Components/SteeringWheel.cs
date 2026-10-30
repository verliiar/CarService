namespace CarService.Domain;

public sealed class SteeringWheel : Component
{
    public SteeringWheel() => Category = ComponentCategory.SteeringWheel;

    public decimal Size
    {
        get; set;
    }

    public string Material { get; set; } = "";

    public string Shape { get; set; } = "";
}
