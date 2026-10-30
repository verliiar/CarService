namespace CarService.Application;

public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, UserDto User);
