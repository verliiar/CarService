using CarService.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CarService.Api.Controllers;

[ApiController, Route("api/auth"), ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class AuthController(IAccountService accounts) : CurrentUserController
{
    [AllowAnonymous, EnableRateLimiting("authentication"), HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await accounts.RegisterAsync(request, ct));

    [AllowAnonymous, EnableRateLimiting("authentication"), HttpPost("login")]
    public Task<AuthResponse> Login(LoginRequest request, CancellationToken ct) => accounts.LoginAsync(request, ct);

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await accounts.LogoutAsync(CurrentUserId, ct);
        return NoContent();
    }

    [HttpGet("me")]
    public Task<UserDto> Me(CancellationToken ct) => accounts.GetAsync(CurrentUserId, ct);
}
