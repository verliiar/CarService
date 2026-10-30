namespace CarService.Domain;

public sealed class User : Entity
{
    public string Name { get; set; } = "";

    public UserRole Role
    {
        get; set;
    }
}
