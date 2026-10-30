namespace CarService.Application;

public record TransmissionDto(string CategoryName, Guid DetailId, string Name, decimal AdditionalCost, string Gearbox, string WheelDrive, string EnergyTransfer);
