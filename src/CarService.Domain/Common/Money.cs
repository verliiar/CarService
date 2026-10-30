namespace CarService.Domain;

public readonly record struct Money
{
    public decimal Value
    {
        get;
    }

    public Money(decimal value)
    {
        if (value < 0 || value > 9999999999999999.99m || decimal.Round(value, 2) != value)
        {
            throw new BusinessException(FailureKind.Validation, "Money must fit decimal(18,2) and be nonnegative.");
        }

        Value = value;
    }
}
