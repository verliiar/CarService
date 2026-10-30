using CarService.Application;
using CarService.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[Authorize(Roles = "WarehouseAdmin,SystemAdmin"), ApiController, Route("api/details/{id:guid}"), Route("api/details")]
public sealed class DetailsController(ComponentService components) : CurrentUserController
{
    [HttpGet("interior")]
    public async Task<List<InteriorDto>> ListInterior(CancellationToken ct) => (await components.ReadAsync(CurrentUserId, ComponentCategory.Interior, null, ct)).Cast<InteriorDto>().ToList();

    [HttpGet("interior/{name}")]
    public async Task<InteriorDto> FindInterior(string name, CancellationToken ct) => (InteriorDto)(await components.ReadAsync(CurrentUserId, ComponentCategory.Interior, name, ct)).Single();

    [HttpPost("interior")]
    public async Task<IActionResult> AddInterior(DetailRequest request, CancellationToken ct)
    {
        await components.SaveAsync(CurrentUserId, ComponentCategory.Interior, request, false, ct);
        return StatusCode(201);
    }

    [HttpPut("interior")]
    public async Task<IActionResult> EditInterior(DetailRequest request, CancellationToken ct)
    {
        await components.SaveAsync(CurrentUserId, ComponentCategory.Interior, request, true, ct);
        return NoContent();
    }

    [Authorize(Roles = "SystemAdmin")]
    [HttpDelete("interior/{name}")]
    public async Task<IActionResult> DeleteInterior(string name, CancellationToken ct)
    {
        await components.DeleteAsync(CurrentUserId, ComponentCategory.Interior, name, ct);
        return NoContent();
    }

    [HttpGet("steering_wheel")]
    public async Task<List<SteeringWheelDto>> ListSteeringWheel(CancellationToken ct) => (await components.ReadAsync(CurrentUserId, ComponentCategory.SteeringWheel, null, ct)).Cast<SteeringWheelDto>().ToList();

    [HttpGet("steering_wheel/{name}")]
    public async Task<SteeringWheelDto> FindSteeringWheel(string name, CancellationToken ct) => (SteeringWheelDto)(await components.ReadAsync(CurrentUserId, ComponentCategory.SteeringWheel, name, ct)).Single();

    [HttpPost("steering_wheel")]
    public async Task<IActionResult> AddSteeringWheel(DetailRequest request, CancellationToken ct)
    {
        await components.SaveAsync(CurrentUserId, ComponentCategory.SteeringWheel, request, false, ct);
        return StatusCode(201);
    }

    [HttpPut("steering_wheel")]
    public async Task<IActionResult> EditSteeringWheel(DetailRequest request, CancellationToken ct)
    {
        await components.SaveAsync(CurrentUserId, ComponentCategory.SteeringWheel, request, true, ct);
        return NoContent();
    }

    [Authorize(Roles = "SystemAdmin")]
    [HttpDelete("steering_wheel/{name}")]
    public async Task<IActionResult> DeleteSteeringWheel(string name, CancellationToken ct)
    {
        await components.DeleteAsync(CurrentUserId, ComponentCategory.SteeringWheel, name, ct);
        return NoContent();
    }

    [HttpGet("transmission")]
    public async Task<List<TransmissionDto>> ListTransmission(CancellationToken ct) => (await components.ReadAsync(CurrentUserId, ComponentCategory.Transmission, null, ct)).Cast<TransmissionDto>().ToList();

    [HttpGet("transmission/{name}")]
    public async Task<TransmissionDto> FindTransmission(string name, CancellationToken ct) => (TransmissionDto)(await components.ReadAsync(CurrentUserId, ComponentCategory.Transmission, name, ct)).Single();

    [HttpPost("transmission")]
    public async Task<IActionResult> AddTransmission(DetailRequest request, CancellationToken ct)
    {
        await components.SaveAsync(CurrentUserId, ComponentCategory.Transmission, request, false, ct);
        return StatusCode(201);
    }

    [HttpPut("transmission")]
    public async Task<IActionResult> EditTransmission(DetailRequest request, CancellationToken ct)
    {
        await components.SaveAsync(CurrentUserId, ComponentCategory.Transmission, request, true, ct);
        return NoContent();
    }

    [Authorize(Roles = "SystemAdmin")]
    [HttpDelete("transmission/{name}")]
    public async Task<IActionResult> DeleteTransmission(string name, CancellationToken ct)
    {
        await components.DeleteAsync(CurrentUserId, ComponentCategory.Transmission, name, ct);
        return NoContent();
    }

    [HttpGet("wheel")]
    public async Task<List<WheelDto>> ListWheel(CancellationToken ct) => (await components.ReadAsync(CurrentUserId, ComponentCategory.Wheel, null, ct)).Cast<WheelDto>().ToList();

    [HttpGet("wheel/{name}")]
    public async Task<WheelDto> FindWheel(string name, CancellationToken ct) => (WheelDto)(await components.ReadAsync(CurrentUserId, ComponentCategory.Wheel, name, ct)).Single();

    [HttpPost("wheel")]
    public async Task<IActionResult> AddWheel(DetailRequest request, CancellationToken ct)
    {
        await components.SaveAsync(CurrentUserId, ComponentCategory.Wheel, request, false, ct);
        return StatusCode(201);
    }

    [HttpPut("wheel")]
    public async Task<IActionResult> EditWheel(DetailRequest request, CancellationToken ct)
    {
        await components.SaveAsync(CurrentUserId, ComponentCategory.Wheel, request, true, ct);
        return NoContent();
    }

    [Authorize(Roles = "SystemAdmin")]
    [HttpDelete("wheel/{name}")]
    public async Task<IActionResult> DeleteWheel(string name, CancellationToken ct)
    {
        await components.DeleteAsync(CurrentUserId, ComponentCategory.Wheel, name, ct);
        return NoContent();
    }
}
