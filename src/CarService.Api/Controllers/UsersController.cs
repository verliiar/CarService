using CarService.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController, Route("api/users")]
public sealed class UsersController(IAccountService accounts) : CurrentUserController
{
    [AllowAnonymous, HttpPost("{role}/{user}")]
    public IActionResult Open(string role, string user) => Problem(statusCode: StatusCodes.Status410Gone,
        title: "Password authentication is required", detail: "Use /api/auth/register or /api/auth/login.");

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Close(CancellationToken ct)
    {
        await accounts.LogoutAsync(CurrentUserId, ct);
        return NoContent();
    }

    [Authorize(Roles = "SystemAdmin"), HttpPost("{userId:guid}/credentials")]
    public async Task<IActionResult> Provision(Guid userId, ProvisionAccountRequest request, CancellationToken ct)
    {
        await accounts.ProvisionAsync(CurrentUserId, userId, request, ct);
        return StatusCode(StatusCodes.Status201Created);
    }

    [Authorize(Roles = "SystemAdmin"), HttpPut("{userId:guid}/role")]
    public async Task<IActionResult> SetRole(Guid userId, ChangeRoleRequest request, CancellationToken ct)
    {
        await accounts.SetRoleAsync(CurrentUserId, userId, request.Role, ct);
        return NoContent();
    }
}
