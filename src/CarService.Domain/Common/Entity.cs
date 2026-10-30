namespace CarService.Domain;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool IsRemoved
    {
        get; set;
    }

    public DateTimeOffset CreatedAt
    {
        get; set;
    }

    public DateTimeOffset UpdatedAt
    {
        get; set;
    }
}
