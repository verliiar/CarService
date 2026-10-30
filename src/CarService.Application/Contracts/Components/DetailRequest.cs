using System.ComponentModel.DataAnnotations;

namespace CarService.Application;

public sealed record DetailRequest
{
    [Required, MaxLength(64)]
    public required string DetailName
    {
        get; init;
    }

    [MaxLength(64)]
    public string? NewDetailName
    {
        get; init;
    }

    [Required]
    public required Dictionary<string, string> SupportedCars
    {
        get; init;
    }

    [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal AdditionalCost
    {
        get; init;
    }

    [MaxLength(64)]
    public string? Material
    {
        get; init;
    }

    [MaxLength(64)]
    public string? Shape
    {
        get; init;
    }

    public decimal Size
    {
        get; init;
    }

    public decimal Diameter
    {
        get; init;
    }

    public int LoadIndex
    {
        get; init;
    }

    public string? Gearbox
    {
        get; init;
    }

    public string? WheelDrive
    {
        get; init;
    }

    public string? EnergyTransfer
    {
        get; init;
    }

    public string? Seasonality
    {
        get; init;
    }
}
