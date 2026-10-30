using CarService.Domain;

namespace CarService.Application;

public record OrderDto(string State, string Type, Guid OrderId, Guid ManagerId, CarSnapshot Car);
