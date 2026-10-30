using CarService.Domain;

namespace CarService.Application;

public static class Rules
{
    public static T Parse<T>(string? value) where T : struct, Enum => value != null && Enum.TryParse<T>(value, false, out var parsed) && Enum.IsDefined(parsed) && parsed.ToString() == value
        ? parsed : throw new BusinessException(FailureKind.Validation, $"Invalid {typeof(T).Name}.");

    public static string Text(string? value) => !string.IsNullOrWhiteSpace(value) && value.Length <= 64 ? value : throw new BusinessException(FailureKind.Validation, "A nonempty value of at most 64 characters is required.");
    public static decimal Measurement(decimal value, bool positive = false) => value >= (positive ? 0.0001m : 0m) && value <= 99999999m && decimal.Round(value, 4) == value
        ? value : throw new BusinessException(FailureKind.Validation, "Invalid measurement or precision.");

    public static T Found<T>(T? value) where T : class => value ?? throw new BusinessException(FailureKind.NotFound, $"{typeof(T).Name} not found.");
}
