namespace CarService.Domain;

public sealed class BusinessException(FailureKind kind, string message) : Exception(message)
{
    public FailureKind Kind { get; } = kind;
}
