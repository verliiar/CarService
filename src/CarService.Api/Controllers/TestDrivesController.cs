using CarService.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController, Route("api/test_drives/{id:guid}"), Route("api/test_drives")]
public sealed class TestDrivesController(TestDriveService drives) : CurrentUserController
{
    [HttpPost("booking")]
    [Authorize(Roles = "Customer,SystemAdmin")]
    public async Task<ActionResult<BookingDto>> Book(BookingRequest request, CancellationToken ct) => StatusCode(201, await drives.SaveAsync(CurrentUserId, null, request, ct));

    [HttpPut("{booking:guid}")]
    [Authorize(Roles = "Customer,SystemAdmin")]
    public async Task<IActionResult> Edit(Guid booking, BookingRequest request, CancellationToken ct)
    {
        await drives.SaveAsync(CurrentUserId, booking, request, ct);
        return NoContent();
    }

    [HttpDelete("{booking:guid}")]
    [Authorize(Roles = "Customer,SystemAdmin")]
    public async Task<IActionResult> Delete(Guid booking, CancellationToken ct)
    {
        await drives.DeleteAsync(CurrentUserId, booking, ct);
        return NoContent();
    }

    [HttpPost("car/{brand}/{model}")]
    [Authorize(Roles = "Manager,SystemAdmin")]
    public async Task<IActionResult> AddCar(string brand, string model, CancellationToken ct)
    {
        await drives.FleetAsync(CurrentUserId, brand, model, false, ct);
        return NoContent();
    }

    [HttpDelete("car/{brand}/{model}")]
    [Authorize(Roles = "Manager,SystemAdmin")]
    public async Task<IActionResult> RemoveCar(string brand, string model, CancellationToken ct)
    {
        await drives.FleetAsync(CurrentUserId, brand, model, true, ct);
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Customer,Manager,SystemAdmin")]
    public Task<List<BookingDto>> List(CancellationToken ct) => drives.ListAsync(CurrentUserId, ct);
}
