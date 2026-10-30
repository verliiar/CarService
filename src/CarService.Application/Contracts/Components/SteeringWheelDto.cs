namespace CarService.Application;

public record SteeringWheelDto(string CategoryName, Guid DetailId, string Name, decimal AdditionalCost, decimal Size, string Material, string Shape);
