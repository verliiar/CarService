namespace CarService.Domain;

public enum FailureKind
{
    Validation,
    NotFound,
    Forbidden,
    Conflict,
    Unauthorized,
}
