using CarService.Domain;

namespace CarService.Application;

public sealed record CarFilter
{
    public decimal? PriceLowerLimit
    {
        get; init;
    }

    public decimal? PriceUpperLimit
    {
        get; init;
    }

    public decimal? PriceEquals
    {
        get; init;
    }

    public string? Brand
    {
        get; init;
    }

    public string? Model
    {
        get; init;
    }

    public string? Body
    {
        get; init;
    }

    public string? Fuel
    {
        get; init;
    }

    public decimal? EnginePowerLowerLimit
    {
        get; init;
    }

    public decimal? EnginePowerUpperLimit
    {
        get; init;
    }

    public decimal? EnginePowerEquals
    {
        get; init;
    }

    public decimal? EngineDisplacementLowerLimit
    {
        get; init;
    }

    public decimal? EngineDisplacementUpperLimit
    {
        get; init;
    }

    public decimal? EngineDisplacementEquals
    {
        get; init;
    }

    public string? Color
    {
        get; init;
    }

    public IQueryable<Car> Apply(IQueryable<Car> query)
    {
        if (Model != null && Brand == null)
        {
            throw new BusinessException(FailureKind.Validation, "Specify brand when filtering by model.");
        }

        ValidateRange(PriceLowerLimit, PriceUpperLimit, PriceEquals);
        ValidateRange(EnginePowerLowerLimit, EnginePowerUpperLimit, EnginePowerEquals);
        ValidateRange(EngineDisplacementLowerLimit, EngineDisplacementUpperLimit, EngineDisplacementEquals);
        var body = Body == null ? (BodyType?)null : Rules.Parse<BodyType>(Body);
        var fuel = Fuel == null ? (FuelType?)null : Rules.Parse<FuelType>(Fuel);
        return query.Where(c => (Brand == null || c.Brand == Brand) && (Model == null || c.Model == Model) &&
            (body == null || c.Body == body) && (fuel == null || c.Fuel == fuel) && (Color == null || c.Color == Color) &&
            (PriceEquals == null || c.Price == PriceEquals) && (PriceLowerLimit == null || (PriceEquals == null ? c.Price > PriceLowerLimit : c.Price >= PriceLowerLimit)) &&
            (PriceUpperLimit == null || (PriceEquals == null ? c.Price < PriceUpperLimit : c.Price <= PriceUpperLimit)) &&
            (EnginePowerEquals == null || c.EnginePower == EnginePowerEquals) && (EnginePowerLowerLimit == null || (EnginePowerEquals == null ? c.EnginePower > EnginePowerLowerLimit : c.EnginePower >= EnginePowerLowerLimit)) &&
            (EnginePowerUpperLimit == null || (EnginePowerEquals == null ? c.EnginePower < EnginePowerUpperLimit : c.EnginePower <= EnginePowerUpperLimit)) &&
            (EngineDisplacementEquals == null || c.EngineDisplacement == EngineDisplacementEquals) &&
            (EngineDisplacementLowerLimit == null || (EngineDisplacementEquals == null ? c.EngineDisplacement > EngineDisplacementLowerLimit : c.EngineDisplacement >= EngineDisplacementLowerLimit)) &&
            (EngineDisplacementUpperLimit == null || (EngineDisplacementEquals == null ? c.EngineDisplacement < EngineDisplacementUpperLimit : c.EngineDisplacement <= EngineDisplacementUpperLimit)));
    }

    private static void ValidateRange(decimal? min, decimal? max, decimal? equal)
    {
        if (min < 0 || max < 0 || equal < 0 || min > max || (equal != null && ((min != null && min != equal) || (max != null && max != equal) || (min != null && max != null))))
        {
            throw new BusinessException(FailureKind.Validation, "Inconsistent filter range.");
        }
    }
}
