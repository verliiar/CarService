#nullable enable
using Microsoft.EntityFrameworkCore.Migrations;



namespace CarService.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    private static readonly string[] CarColumns = ["Id", "Body", "Brand", "Color", "CreatedAt", "EngineDisplacement", "EnginePower", "Fuel", "IsAvailable", "IsRemoved", "Model", "Price", "UpdatedAt"];
    private static readonly string[] CategoryColumns = ["Id", "Name"];
    private static readonly string[] UserColumns = ["Id", "CreatedAt", "IsRemoved", "Name", "Role", "UpdatedAt"];
    private static readonly string[] ComponentColumns = ["Id", "AdditionalCost", "Category", "CreatedAt", "IsRemoved", "Name", "UpdatedAt"];
    private static readonly string[] OrderColumns = ["Id", "CarId", "Configuration", "CreatedAt", "CustomerId", "IsRemoved", "ManagerId", "State", "Type", "UpdatedAt"];
    private static readonly string[] BookingColumns = ["Id", "BookerId", "BookingDateTime", "CarId", "CreatedAt", "IsRemoved", "UpdatedAt"];
    private static readonly string[] CarComponentColumns = ["CarId", "ComponentId"];
    private static readonly string[] InteriorColumns = ["Id", "Material"];
    private static readonly string[] SteeringWheelColumns = ["Id", "Material", "Shape", "Size"];
    private static readonly string[] TransmissionColumns = ["Id", "EnergyTransfer", "Gearbox", "WheelDrive"];
    private static readonly string[] WheelColumns = ["Id", "Diameter", "LoadIndex", "Seasonality"];
    private static readonly string[] CarIdentityColumns = ["Brand", "Model"];
    private static readonly string[] ComponentIdentityColumns = ["Category", "Name"];
    private static readonly string[] BookingSlotColumns = ["CarId", "BookingDateTime"];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "cars",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Brand = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Model = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Body = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Fuel = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                EnginePower = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                EngineDisplacement = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                Color = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_cars", x => x.Id);
                table.CheckConstraint("car_engine", "\"EnginePower\" >= 0 AND \"EngineDisplacement\" >= 0");
                table.CheckConstraint("car_price", "\"Price\" >= 0");
            });

        migrationBuilder.CreateTable(
            name: "components_categories",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_components_categories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "test_drive_cars",
            columns: table => new
            {
                CarId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_test_drive_cars", x => x.CarId);
                table.ForeignKey(
                    name: "FK_test_drive_cars_cars_CarId",
                    column: x => x.CarId,
                    principalTable: "cars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "components",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                AdditionalCost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_components", x => x.Id);
                table.CheckConstraint("component_price", "\"AdditionalCost\" >= 0");
                table.ForeignKey(
                    name: "FK_components_components_categories_Category",
                    column: x => x.Category,
                    principalTable: "components_categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "orders",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                ManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                CarId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                State = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Configuration = table.Column<string>(type: "jsonb", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_orders", x => x.Id);
                table.ForeignKey(
                    name: "FK_orders_cars_CarId",
                    column: x => x.CarId,
                    principalTable: "cars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_orders_users_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_orders_users_ManagerId",
                    column: x => x.ManagerId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "test_drive_bookings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BookerId = table.Column<Guid>(type: "uuid", nullable: false),
                CarId = table.Column<Guid>(type: "uuid", nullable: false),
                BookingDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_test_drive_bookings", x => x.Id);
                table.ForeignKey(
                    name: "FK_test_drive_bookings_cars_CarId",
                    column: x => x.CarId,
                    principalTable: "cars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_test_drive_bookings_users_BookerId",
                    column: x => x.BookerId,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "car_components",
            columns: table => new
            {
                CarId = table.Column<Guid>(type: "uuid", nullable: false),
                ComponentId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_car_components", x => new { x.CarId, x.ComponentId });
                table.ForeignKey(
                    name: "FK_car_components_cars_CarId",
                    column: x => x.CarId,
                    principalTable: "cars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_car_components_components_ComponentId",
                    column: x => x.ComponentId,
                    principalTable: "components",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "compatibilities",
            columns: table => new
            {
                CarId = table.Column<Guid>(type: "uuid", nullable: false),
                ComponentId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_compatibilities", x => new { x.CarId, x.ComponentId });
                table.ForeignKey(
                    name: "FK_compatibilities_cars_CarId",
                    column: x => x.CarId,
                    principalTable: "cars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_compatibilities_components_ComponentId",
                    column: x => x.ComponentId,
                    principalTable: "components",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "interiors",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Material = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_interiors", x => x.Id);
                table.ForeignKey(
                    name: "FK_interiors_components_Id",
                    column: x => x.Id,
                    principalTable: "components",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "steering_wheels",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Size = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                Material = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Shape = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_steering_wheels", x => x.Id);
                table.CheckConstraint("steering_size", "\"Size\" > 0");
                table.ForeignKey(
                    name: "FK_steering_wheels_components_Id",
                    column: x => x.Id,
                    principalTable: "components",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "transmissions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Gearbox = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                WheelDrive = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                EnergyTransfer = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_transmissions", x => x.Id);
                table.ForeignKey(
                    name: "FK_transmissions_components_Id",
                    column: x => x.Id,
                    principalTable: "components",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "wheels",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Diameter = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                LoadIndex = table.Column<int>(type: "integer", nullable: false),
                Seasonality = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_wheels", x => x.Id);
                table.CheckConstraint("wheel_dimensions", "\"Diameter\" > 0 AND \"LoadIndex\" > 0");
                table.ForeignKey(
                    name: "FK_wheels_components_Id",
                    column: x => x.Id,
                    principalTable: "components",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "cars",
            columns: CarColumns,
            values: new object?[,]
            {
                { new Guid("11111111-1111-1111-1111-111111111111"), "Sedan", "BMW", "Black", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.0m, 184m, "Petrol", true, false, "320i", 3500000m, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("22222222-2222-2222-2222-222222222222"), "Sedan", "BMW", "White", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.0m, 258m, "Petrol", true, false, "330i", 4200000m, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("33333333-3333-3333-3333-333333333333"), "Sedan", "Audi", "Grey", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.0m, 190m, "Diesel", false, false, "A4", 3000000m, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("44444444-4444-4444-4444-444444444444"), "Sedan", "Toyota", "Red", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 203m, "Petrol", true, false, "Camry", 2700000m, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("55555555-5555-5555-5555-555555555555"), "Wagon", "Volkswagen", "Black", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.0m, 190m, "Diesel", true, false, "Passat", 3200000m, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
            });

        migrationBuilder.InsertData(
            table: "components_categories",
            columns: CategoryColumns,
            values: new object?[,]
            {
                { "Interior", "Interior" },
                { "SteeringWheel", "SteeringWheel" },
                { "Transmission", "Transmission" },
                { "Wheel", "Wheel" }
            });

        migrationBuilder.InsertData(
            table: "users",
            columns: UserColumns,
            values: new object?[,]
            {
                { new Guid("11111111-1111-1111-1111-111111111111"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Masha", "Customer", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("22222222-2222-2222-2222-222222222222"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Gleb", "Customer", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("33333333-3333-3333-3333-333333333333"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Sasha", "Manager", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("44444444-4444-4444-4444-444444444444"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Daniil", "WarehouseAdmin", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("55555555-5555-5555-5555-555555555555"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Nahid", "SystemAdmin", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
            });

        migrationBuilder.InsertData(
            table: "components",
            columns: ComponentColumns,
            values: new object?[,]
            {
                { new Guid("11111111-1111-1111-1111-111111111111"), 7000m, "Wheel", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "17’’ Standard", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("22222222-2222-2222-2222-222222222222"), 95000m, "Wheel", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "19’’ M-Sport", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("33333333-3333-3333-3333-333333333333"), 45000m, "Wheel", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "18’’ Aero", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("44444444-4444-4444-4444-444444444444"), 5000m, "Transmission", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Автоматическая 8AT", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("55555555-5555-5555-5555-555555555555"), 30000m, "Transmission", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Механическая 6MT", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("66666666-6666-6666-6666-666666666666"), 5000m, "SteeringWheel", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Спортивный кожаный (Standard)", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("77777777-7777-7777-7777-777777777777"), 25000m, "SteeringWheel", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "M-Sport с подогревом", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("88888888-8888-8888-8888-888888888888"), 10000m, "Interior", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Тканевый Graphite", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("99999999-9999-9999-9999-999999999999"), 110000m, "Interior", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, "Кожаный Dakota", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
            });

        migrationBuilder.InsertData(
            table: "orders",
            columns: OrderColumns,
            values: new object?[,]
            {
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "{\"Brand\":\"BMW\",\"Model\":\"320i\",\"Price\":3500000,\"RequiredDetails\":[{\"CategoryName\":\"Interior\",\"Name\":\"\\u0422\\u043A\\u0430\\u043D\\u0435\\u0432\\u044B\\u0439 Graphite\"},{\"CategoryName\":\"SteeringWheel\",\"Name\":\"\\u0421\\u043F\\u043E\\u0440\\u0442\\u0438\\u0432\\u043D\\u044B\\u0439 \\u043A\\u043E\\u0436\\u0430\\u043D\\u044B\\u0439 (Standard)\"},{\"CategoryName\":\"Transmission\",\"Name\":\"\\u0410\\u0432\\u0442\\u043E\\u043C\\u0430\\u0442\\u0438\\u0447\\u0435\\u0441\\u043A\\u0430\\u044F 8AT\"},{\"CategoryName\":\"Wheel\",\"Name\":\"17\\u2019\\u2019 Standard\"}],\"Body\":\"Sedan\",\"Engine\":{\"Fuel\":\"Petrol\",\"Power\":184,\"Displacement\":2.0},\"Color\":\"Black\",\"IsAvailable\":true}", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), false, new Guid("33333333-3333-3333-3333-333333333333"), "Registered", "Stock", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("33333333-3333-3333-3333-333333333333"), "{\"Brand\":\"Audi\",\"Model\":\"A4\",\"Price\":3000000,\"RequiredDetails\":[{\"CategoryName\":\"Interior\",\"Name\":\"\\u0422\\u043A\\u0430\\u043D\\u0435\\u0432\\u044B\\u0439 Graphite\"},{\"CategoryName\":\"SteeringWheel\",\"Name\":\"M-Sport \\u0441 \\u043F\\u043E\\u0434\\u043E\\u0433\\u0440\\u0435\\u0432\\u043E\\u043C\"},{\"CategoryName\":\"Transmission\",\"Name\":\"\\u041C\\u0435\\u0445\\u0430\\u043D\\u0438\\u0447\\u0435\\u0441\\u043A\\u0430\\u044F 6MT\"},{\"CategoryName\":\"Wheel\",\"Name\":\"18\\u2019\\u2019 Aero\"}],\"Body\":\"Sedan\",\"Engine\":{\"Fuel\":\"Diesel\",\"Power\":190,\"Displacement\":2.0},\"Color\":\"Grey\",\"IsAvailable\":false}", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), false, new Guid("33333333-3333-3333-3333-333333333333"), "AwaitingPayment", "Custom", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("44444444-4444-4444-4444-444444444444"), "{\"Brand\":\"Toyota\",\"Model\":\"Camry\",\"Price\":2700000,\"RequiredDetails\":[{\"CategoryName\":\"Interior\",\"Name\":\"\\u0422\\u043A\\u0430\\u043D\\u0435\\u0432\\u044B\\u0439 Graphite\"},{\"CategoryName\":\"SteeringWheel\",\"Name\":\"\\u0421\\u043F\\u043E\\u0440\\u0442\\u0438\\u0432\\u043D\\u044B\\u0439 \\u043A\\u043E\\u0436\\u0430\\u043D\\u044B\\u0439 (Standard)\"},{\"CategoryName\":\"Transmission\",\"Name\":\"\\u0410\\u0432\\u0442\\u043E\\u043C\\u0430\\u0442\\u0438\\u0447\\u0435\\u0441\\u043A\\u0430\\u044F 8AT\"},{\"CategoryName\":\"Wheel\",\"Name\":\"17\\u2019\\u2019 Standard\"}],\"Body\":\"Sedan\",\"Engine\":{\"Fuel\":\"Petrol\",\"Power\":203,\"Displacement\":2.5},\"Color\":\"Red\",\"IsAvailable\":true}", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), false, new Guid("33333333-3333-3333-3333-333333333333"), "Completed", "Stock", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
            });

        migrationBuilder.InsertData(
            table: "test_drive_bookings",
            columns: BookingColumns,
            values: new object?[,]
            {
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("11111111-1111-1111-1111-111111111111"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
            });

        migrationBuilder.InsertData(
            table: "test_drive_cars",
            column: "CarId",
            values: new object[]
            {
                new Guid("11111111-1111-1111-1111-111111111111"),
                new Guid("22222222-2222-2222-2222-222222222222"),
                new Guid("44444444-4444-4444-4444-444444444444")
            });

        migrationBuilder.InsertData(
            table: "car_components",
            columns: CarComponentColumns,
            values: new object?[,]
            {
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111") },
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("66666666-6666-6666-6666-666666666666") },
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888888") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("66666666-6666-6666-6666-666666666666") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("99999999-9999-9999-9999-999999999999") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("33333333-3333-3333-3333-333333333333") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("55555555-5555-5555-5555-555555555555") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("77777777-7777-7777-7777-777777777777") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888888") },
                { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("11111111-1111-1111-1111-111111111111") },
                { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("66666666-6666-6666-6666-666666666666") },
                { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("88888888-8888-8888-8888-888888888888") },
                { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("22222222-2222-2222-2222-222222222222") },
                { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("77777777-7777-7777-7777-777777777777") },
                { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("99999999-9999-9999-9999-999999999999") }
            });

        migrationBuilder.InsertData(
            table: "compatibilities",
            columns: CarComponentColumns,
            values: new object?[,]
            {
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111") },
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222") },
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("55555555-5555-5555-5555-555555555555") },
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("66666666-6666-6666-6666-666666666666") },
                { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888888") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("22222222-2222-2222-2222-222222222222") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("55555555-5555-5555-5555-555555555555") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("66666666-6666-6666-6666-666666666666") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("77777777-7777-7777-7777-777777777777") },
                { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("99999999-9999-9999-9999-999999999999") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("33333333-3333-3333-3333-333333333333") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("55555555-5555-5555-5555-555555555555") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("77777777-7777-7777-7777-777777777777") },
                { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888888") },
                { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("11111111-1111-1111-1111-111111111111") },
                { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("66666666-6666-6666-6666-666666666666") },
                { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("88888888-8888-8888-8888-888888888888") },
                { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("22222222-2222-2222-2222-222222222222") },
                { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("44444444-4444-4444-4444-444444444444") },
                { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("77777777-7777-7777-7777-777777777777") },
                { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("99999999-9999-9999-9999-999999999999") }
            });

        migrationBuilder.InsertData(
            table: "interiors",
            columns: InteriorColumns,
            values: new object?[,]
            {
                { new Guid("88888888-8888-8888-8888-888888888888"), "Fabric" },
                { new Guid("99999999-9999-9999-9999-999999999999"), "Leather" }
            });

        migrationBuilder.InsertData(
            table: "steering_wheels",
            columns: SteeringWheelColumns,
            values: new object?[,]
            {
                { new Guid("66666666-6666-6666-6666-666666666666"), "Leather", "Round", 38m },
                { new Guid("77777777-7777-7777-7777-777777777777"), "Leather", "Round", 36m }
            });

        migrationBuilder.InsertData(
            table: "transmissions",
            columns: TransmissionColumns,
            values: new object?[,]
            {
                { new Guid("44444444-4444-4444-4444-444444444444"), "Mechanical", "Automatic", "All" },
                { new Guid("55555555-5555-5555-5555-555555555555"), "Mechanical", "Manual", "All" }
            });

        migrationBuilder.InsertData(
            table: "wheels",
            columns: WheelColumns,
            values: new object?[,]
            {
                { new Guid("11111111-1111-1111-1111-111111111111"), 17m, 91, "All" },
                { new Guid("22222222-2222-2222-2222-222222222222"), 19m, 95, "Summer" },
                { new Guid("33333333-3333-3333-3333-333333333333"), 18m, 93, "Winter" }
            });

        migrationBuilder.CreateIndex(
            name: "IX_car_components_ComponentId",
            table: "car_components",
            column: "ComponentId");

        migrationBuilder.CreateIndex(
            name: "IX_cars_Brand_Model",
            table: "cars",
            columns: CarIdentityColumns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_cars_IsAvailable",
            table: "cars",
            column: "IsAvailable");

        migrationBuilder.CreateIndex(
            name: "IX_compatibilities_ComponentId",
            table: "compatibilities",
            column: "ComponentId");

        migrationBuilder.CreateIndex(
            name: "IX_components_Category_Name",
            table: "components",
            columns: ComponentIdentityColumns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_components_categories_Name",
            table: "components_categories",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_orders_CarId",
            table: "orders",
            column: "CarId");

        migrationBuilder.CreateIndex(
            name: "IX_orders_CustomerId",
            table: "orders",
            column: "CustomerId");

        migrationBuilder.CreateIndex(
            name: "IX_orders_ManagerId",
            table: "orders",
            column: "ManagerId");

        migrationBuilder.CreateIndex(
            name: "IX_test_drive_bookings_BookerId",
            table: "test_drive_bookings",
            column: "BookerId");

        migrationBuilder.CreateIndex(
            name: "IX_test_drive_bookings_CarId_BookingDateTime",
            table: "test_drive_bookings",
            columns: BookingSlotColumns,
            unique: true,
            filter: "NOT \"IsRemoved\"");

        migrationBuilder.CreateIndex(
            name: "IX_users_Name",
            table: "users",
            column: "Name",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "car_components");

        migrationBuilder.DropTable(
            name: "compatibilities");

        migrationBuilder.DropTable(
            name: "interiors");

        migrationBuilder.DropTable(
            name: "orders");

        migrationBuilder.DropTable(
            name: "steering_wheels");

        migrationBuilder.DropTable(
            name: "test_drive_bookings");

        migrationBuilder.DropTable(
            name: "test_drive_cars");

        migrationBuilder.DropTable(
            name: "transmissions");

        migrationBuilder.DropTable(
            name: "wheels");

        migrationBuilder.DropTable(
            name: "users");

        migrationBuilder.DropTable(
            name: "cars");

        migrationBuilder.DropTable(
            name: "components");

        migrationBuilder.DropTable(
            name: "components_categories");
    }
}
