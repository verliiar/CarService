using System.ComponentModel.DataAnnotations;

namespace CarService.Application;

public sealed record CarRequest
{
    [Required, MinLength(1)]
    public required List<string> DetailsName
    {
        get; init;
    }

    [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Price
    {
        get; init;
    }

    [Required, MaxLength(64)]
    public required string Brand
    {
        get; init;
    }

    [Required, MaxLength(64)]
    public required string Model
    {
        get; init;
    }

    [MaxLength(64)]
    public string? NewModelName
    {
        get; init;
    }

    [Required]
    public required string Body
    {
        get; init;
    }

    [Required]
    public required string Fuel
    {
        get; init;
    }

    [Range(typeof(decimal), "0", "99999999")]
    public decimal EnginePower
    {
        get; init;
    }

    [Range(typeof(decimal), "0", "99999999")]
    public decimal EngineDisplacement
    {
        get; init;
    }

    [Required, MaxLength(64)]
    public required string Color
    {
        get; init;
    }

    public bool IsAvailable
    {
        get; init;
    }
}
