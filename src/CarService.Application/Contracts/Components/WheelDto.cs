namespace CarService.Application;

public record WheelDto(string CategoryName, Guid DetailId, string Name, decimal AdditionalCost, decimal Diameter, int LoadIndex, string Seasonality);
