namespace CarService.Domain;

public sealed class TestDriveBooking : Entity
{
    public Guid BookerId
    {
        get; set;
    }

    public Guid CarId
    {
        get; set;
    }

    public Car Car { get; set; } = null!;

    public DateTimeOffset BookingDateTime
    {
        get; set;
    }

    public uint Version
    {
        get; set;
    }

    public static DateTimeOffset ValidateDate(DateTimeOffset date, DateTimeOffset now)
    {
        if (date <= now)
        {
            throw new BusinessException(FailureKind.Validation, "A test drive must be in the future.");
        }

        return date.ToUniversalTime();
    }
}
