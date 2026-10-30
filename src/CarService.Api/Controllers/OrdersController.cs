using CarService.Application;
using CarService.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController, Route("api/orders/{id:guid}"), Route("api/orders")]
public sealed class OrdersController(OrderService orders) : CurrentUserController
{
    [HttpPost("stock/{brand}/{model}")]
    [Authorize(Roles = "Customer,SystemAdmin")]
    public async Task<ActionResult<OrderDto>> Stock(string brand, string model, CancellationToken ct) => StatusCode(201, await orders.CreateAsync(CurrentUserId, brand, model, null, ct));

    [HttpPost("custom/{brand}/{model}")]
    [HttpPost("custom{brand}/{model}")]
    [Authorize(Roles = "Customer,SystemAdmin")]
    public async Task<ActionResult<OrderDto>> Custom(string brand, string model, [FromBody] List<string> customOptions, CancellationToken ct) => StatusCode(201, await orders.CreateAsync(CurrentUserId, brand, model, customOptions, ct));

    [HttpPatch("{order:guid}/next_state")]
    public async Task<IActionResult> Advance(Guid order, CancellationToken ct)
    {
        await orders.ChangeAsync(CurrentUserId, order, false, ct);
        return NoContent();
    }

    [HttpPatch("{order:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid order, CancellationToken ct)
    {
        await orders.ChangeAsync(CurrentUserId, order, true, ct);
        return NoContent();
    }

    [HttpGet("{order:guid}")]
    public Task<OrderDto> Read(Guid order, CancellationToken ct) => orders.ReadAsync(CurrentUserId, order, ct);

    [HttpGet("stock")]
    public Task<List<OrderDto>> StockList(CancellationToken ct) => orders.ListAsync(CurrentUserId, OrderType.Stock, ct);

    [HttpGet("custom")]
    public Task<List<OrderDto>> CustomList(CancellationToken ct) => orders.ListAsync(CurrentUserId, OrderType.Custom, ct);

    [HttpDelete("{order:guid}")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> Delete(Guid order, CancellationToken ct)
    {
        await orders.DeleteAsync(CurrentUserId, order, ct);
        return NoContent();
    }
}
