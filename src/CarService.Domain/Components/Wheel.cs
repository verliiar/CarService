namespace CarService.Domain;

public sealed class Wheel : Component
{
    public Wheel() => Category = ComponentCategory.Wheel;

    public decimal Diameter
    {
        get; set;
    }

    public int LoadIndex
    {
        get; set;
    }

    public SeasonalityType Seasonality
    {
        get; set;
    }
}
