namespace CarService.Application;

public interface IAccountService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task LogoutAsync(Guid userId, CancellationToken ct);
    Task ProvisionAsync(Guid actorId, Guid userId, ProvisionAccountRequest request, CancellationToken ct);
    Task SetRoleAsync(Guid actorId, Guid userId, string role, CancellationToken ct);
    Task<UserDto> GetAsync(Guid userId, CancellationToken ct);
}
