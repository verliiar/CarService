using CarService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure;

internal static class SeedData
{
    public static void Seed(this ModelBuilder b)
    {
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        b.Entity<ComponentCategoryEntry>().HasData(new ComponentCategoryEntry { Id = ComponentCategory.Interior, Name = "Interior" });
        b.Entity<ComponentCategoryEntry>().HasData(new ComponentCategoryEntry { Id = ComponentCategory.SteeringWheel, Name = "SteeringWheel" });
        b.Entity<ComponentCategoryEntry>().HasData(new ComponentCategoryEntry { Id = ComponentCategory.Transmission, Name = "Transmission" });
        b.Entity<ComponentCategoryEntry>().HasData(new ComponentCategoryEntry { Id = ComponentCategory.Wheel, Name = "Wheel" });
        b.Entity<User>().HasData(new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Masha", Role = UserRole.Customer });
        b.Entity<User>().HasData(new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Gleb", Role = UserRole.Customer });
        b.Entity<User>().HasData(new User { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Sasha", Role = UserRole.Manager });
        b.Entity<User>().HasData(new User { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Daniil", Role = UserRole.WarehouseAdmin });
        b.Entity<User>().HasData(new User { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Nahid", Role = UserRole.SystemAdmin });
        b.Entity<Car>().HasData(new Car { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), CreatedAt = timestamp, UpdatedAt = timestamp, Brand = "BMW", Model = "320i", Price = 3500000m, Body = BodyType.Sedan, Fuel = FuelType.Petrol, EnginePower = 184m, EngineDisplacement = 2.0m, Color = "Black", IsAvailable = true });
        b.Entity<Car>().HasData(new Car { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), CreatedAt = timestamp, UpdatedAt = timestamp, Brand = "BMW", Model = "330i", Price = 4200000m, Body = BodyType.Sedan, Fuel = FuelType.Petrol, EnginePower = 258m, EngineDisplacement = 2.0m, Color = "White", IsAvailable = true });
        b.Entity<Car>().HasData(new Car { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), CreatedAt = timestamp, UpdatedAt = timestamp, Brand = "Audi", Model = "A4", Price = 3000000m, Body = BodyType.Sedan, Fuel = FuelType.Diesel, EnginePower = 190m, EngineDisplacement = 2.0m, Color = "Grey", IsAvailable = false });
        b.Entity<Car>().HasData(new Car { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), CreatedAt = timestamp, UpdatedAt = timestamp, Brand = "Toyota", Model = "Camry", Price = 2700000m, Body = BodyType.Sedan, Fuel = FuelType.Petrol, EnginePower = 203m, EngineDisplacement = 2.5m, Color = "Red", IsAvailable = true });
        b.Entity<Car>().HasData(new Car { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), CreatedAt = timestamp, UpdatedAt = timestamp, Brand = "Volkswagen", Model = "Passat", Price = 3200000m, Body = BodyType.Wagon, Fuel = FuelType.Diesel, EnginePower = 190m, EngineDisplacement = 2.0m, Color = "Black", IsAvailable = true });
        b.Entity<Wheel>().HasData(new Wheel { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "17’’ Standard", AdditionalCost = 7000m, Diameter = 17m, LoadIndex = 91, Seasonality = SeasonalityType.All });
        b.Entity<Wheel>().HasData(new Wheel { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "19’’ M-Sport", AdditionalCost = 95000m, Diameter = 19m, LoadIndex = 95, Seasonality = SeasonalityType.Summer });
        b.Entity<Wheel>().HasData(new Wheel { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "18’’ Aero", AdditionalCost = 45000m, Diameter = 18m, LoadIndex = 93, Seasonality = SeasonalityType.Winter });
        b.Entity<Transmission>().HasData(new Transmission { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Автоматическая 8AT", AdditionalCost = 5000m, Gearbox = GearboxType.Automatic, WheelDrive = WheelDriveType.All, EnergyTransfer = EnergyTransferType.Mechanical });
        b.Entity<Transmission>().HasData(new Transmission { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Механическая 6MT", AdditionalCost = 30000m, Gearbox = GearboxType.Manual, WheelDrive = WheelDriveType.All, EnergyTransfer = EnergyTransferType.Mechanical });
        b.Entity<SteeringWheel>().HasData(new SteeringWheel { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Спортивный кожаный (Standard)", AdditionalCost = 5000m, Material = "Leather", Shape = "Round", Size = 38m });
        b.Entity<SteeringWheel>().HasData(new SteeringWheel { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "M-Sport с подогревом", AdditionalCost = 25000m, Material = "Leather", Shape = "Round", Size = 36m });
        b.Entity<Interior>().HasData(new Interior { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Тканевый Graphite", AdditionalCost = 10000m, Material = "Fabric" });
        b.Entity<Interior>().HasData(new Interior { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), CreatedAt = timestamp, UpdatedAt = timestamp, Name = "Кожаный Dakota", AdditionalCost = 110000m, Material = "Leather" });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("88888888-8888-8888-8888-888888888888")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("66666666-6666-6666-6666-666666666666")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("99999999-9999-9999-9999-999999999999")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("66666666-6666-6666-6666-666666666666")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("88888888-8888-8888-8888-888888888888")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("77777777-7777-7777-7777-777777777777")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("55555555-5555-5555-5555-555555555555")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("33333333-3333-3333-3333-333333333333")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ComponentId = Guid.Parse("88888888-8888-8888-8888-888888888888")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ComponentId = Guid.Parse("66666666-6666-6666-6666-666666666666")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ComponentId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ComponentId = Guid.Parse("99999999-9999-9999-9999-999999999999")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ComponentId = Guid.Parse("77777777-7777-7777-7777-777777777777")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("car_components").HasData(new
        {
            CarId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ComponentId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("55555555-5555-5555-5555-555555555555")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("66666666-6666-6666-6666-666666666666")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ComponentId = Guid.Parse("88888888-8888-8888-8888-888888888888")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("55555555-5555-5555-5555-555555555555")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("66666666-6666-6666-6666-666666666666")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("77777777-7777-7777-7777-777777777777")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ComponentId = Guid.Parse("99999999-9999-9999-9999-999999999999")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("33333333-3333-3333-3333-333333333333")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("55555555-5555-5555-5555-555555555555")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("77777777-7777-7777-7777-777777777777")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ComponentId = Guid.Parse("88888888-8888-8888-8888-888888888888")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ComponentId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ComponentId = Guid.Parse("66666666-6666-6666-6666-666666666666")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ComponentId = Guid.Parse("88888888-8888-8888-8888-888888888888")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ComponentId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ComponentId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ComponentId = Guid.Parse("77777777-7777-7777-7777-777777777777")
        });
        b.SharedTypeEntity<Dictionary<string, object>>("compatibilities").HasData(new
        {
            CarId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ComponentId = Guid.Parse("99999999-9999-9999-9999-999999999999")
        });
        b.Entity<TestDriveCar>().HasData(new TestDriveCar { CarId = Guid.Parse("11111111-1111-1111-1111-111111111111") });
        b.Entity<TestDriveCar>().HasData(new TestDriveCar { CarId = Guid.Parse("22222222-2222-2222-2222-222222222222") });
        b.Entity<TestDriveCar>().HasData(new TestDriveCar { CarId = Guid.Parse("44444444-4444-4444-4444-444444444444") });
        b.Entity<Order>().HasData(new Order { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), CreatedAt = timestamp, UpdatedAt = timestamp, CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"), ManagerId = Guid.Parse("33333333-3333-3333-3333-333333333333"), CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"), State = OrderState.Registered, Type = OrderType.Stock, Configuration = new CarSnapshot("BMW", "320i", 3500000m, [new DetailSnapshot("Interior", "Тканевый Graphite"), new DetailSnapshot("SteeringWheel", "Спортивный кожаный (Standard)"), new DetailSnapshot("Transmission", "Автоматическая 8AT"), new DetailSnapshot("Wheel", "17’’ Standard")], "Sedan", new EngineSnapshot("Petrol", 184m, 2.0m), "Black", true) });
        b.Entity<Order>().HasData(new Order { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), CreatedAt = timestamp, UpdatedAt = timestamp, CustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222"), ManagerId = Guid.Parse("33333333-3333-3333-3333-333333333333"), CarId = Guid.Parse("33333333-3333-3333-3333-333333333333"), State = OrderState.AwaitingPayment, Type = OrderType.Custom, Configuration = new CarSnapshot("Audi", "A4", 3000000m, [new DetailSnapshot("Interior", "Тканевый Graphite"), new DetailSnapshot("SteeringWheel", "M-Sport с подогревом"), new DetailSnapshot("Transmission", "Механическая 6MT"), new DetailSnapshot("Wheel", "18’’ Aero")], "Sedan", new EngineSnapshot("Diesel", 190m, 2.0m), "Grey", false) });
        b.Entity<Order>().HasData(new Order { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), CreatedAt = timestamp, UpdatedAt = timestamp, CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"), ManagerId = Guid.Parse("33333333-3333-3333-3333-333333333333"), CarId = Guid.Parse("44444444-4444-4444-4444-444444444444"), State = OrderState.Completed, Type = OrderType.Stock, Configuration = new CarSnapshot("Toyota", "Camry", 2700000m, [new DetailSnapshot("Interior", "Тканевый Graphite"), new DetailSnapshot("SteeringWheel", "Спортивный кожаный (Standard)"), new DetailSnapshot("Transmission", "Автоматическая 8AT"), new DetailSnapshot("Wheel", "17’’ Standard")], "Sedan", new EngineSnapshot("Petrol", 203m, 2.5m), "Red", true) });
        b.Entity<TestDriveBooking>().HasData(new TestDriveBooking { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), CreatedAt = timestamp, UpdatedAt = timestamp, BookerId = Guid.Parse("11111111-1111-1111-1111-111111111111"), CarId = Guid.Parse("11111111-1111-1111-1111-111111111111"), BookingDateTime = timestamp });
        b.Entity<TestDriveBooking>().HasData(new TestDriveBooking { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), CreatedAt = timestamp, UpdatedAt = timestamp, BookerId = Guid.Parse("22222222-2222-2222-2222-222222222222"), CarId = Guid.Parse("22222222-2222-2222-2222-222222222222"), BookingDateTime = timestamp });
    }
}
