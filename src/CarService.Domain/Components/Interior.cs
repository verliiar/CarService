namespace CarService.Domain;

public sealed class Interior : Component
{
    public Interior() => Category = ComponentCategory.Interior;

    public string Material { get; set; } = "";
}
