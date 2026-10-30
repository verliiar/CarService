namespace CarService.Domain;

public sealed class ComponentCategoryEntry
{
    public ComponentCategory Id
    {
        get; set;
    }

    public string Name { get; set; } = "";
}
