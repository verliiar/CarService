using CarService.Application;
using CarService.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController, Route("api/cars/{id:guid}"), Route("api/cars")]
public sealed class CarsController(CarCatalog cars) : CurrentUserController
{
    [AllowAnonymous, HttpGet("{brand}/{model}")]
    public async Task<CarSnapshot> Info(string brand, string model, CancellationToken ct) => CarSnapshot.From(await cars.FindAsync(brand, model, ct, false));

    [HttpGet]
    [Authorize(Roles = "WarehouseAdmin,SystemAdmin")]
    public Task<List<CarSnapshot>> List(CancellationToken ct) => cars.ListAsync(CurrentUserId, false, null, null, null, ct);

    [HttpGet("available")]
    [Authorize(Roles = "Customer,Manager,SystemAdmin")]
    public Task<List<CarSnapshot>> Available(CancellationToken ct) => cars.ListAsync(CurrentUserId, true, null, null, null, ct);

    [HttpGet("available/filtered")]
    [HttpPost("available/search")]
    [Authorize(Roles = "Customer,Manager,SystemAdmin")]
    public Task<List<CarSnapshot>> Filter([FromQuery] string? brand, [FromQuery] List<string>? details,
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] CarFilter? filter, CancellationToken ct) => cars.ListAsync(CurrentUserId, true, filter, brand, details, ct);

    [HttpPost]
    [Authorize(Roles = "WarehouseAdmin,SystemAdmin")]
    public async Task<IActionResult> Add(CarRequest request, CancellationToken ct)
    {
        await cars.SaveAsync(CurrentUserId, request, false, ct);
        return StatusCode(201);
    }

    [HttpPut]
    [Authorize(Roles = "WarehouseAdmin,SystemAdmin")]
    public async Task<IActionResult> Edit(CarRequest request, CancellationToken ct)
    {
        await cars.SaveAsync(CurrentUserId, request, true, ct);
        return NoContent();
    }

    [HttpDelete("{brand}/{model}")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> Delete(string brand, string model, CancellationToken ct)
    {
        await cars.DeleteAsync(CurrentUserId, brand, model, ct);
        return NoContent();
    }
}
