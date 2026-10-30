#nullable enable
using Microsoft.EntityFrameworkCore.Migrations;


namespace CarService.Infrastructure.Migrations;

public partial class EnumConstraints : Migration
{
    private static readonly string[] OrderTypeIndexColumns = ["Type", "IsRemoved"];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddCheckConstraint(
            name: "valid_seasonality",
            table: "wheels",
            sql: "\"Seasonality\" IN ('Summer','Winter','All')");

        migrationBuilder.AddCheckConstraint(
            name: "valid_user_role",
            table: "users",
            sql: "\"Role\" IN ('Manager','WarehouseAdmin','Customer','SystemAdmin')");

        migrationBuilder.AddCheckConstraint(
            name: "valid_energy_transfer",
            table: "transmissions",
            sql: "\"EnergyTransfer\" IN ('Mechanical','Hydromechanical','Hydrostatic')");

        migrationBuilder.AddCheckConstraint(
            name: "valid_gearbox",
            table: "transmissions",
            sql: "\"Gearbox\" IN ('Manual','Automatic')");

        migrationBuilder.AddCheckConstraint(
            name: "valid_wheel_drive",
            table: "transmissions",
            sql: "\"WheelDrive\" IN ('Front','Rear','All')");

        migrationBuilder.CreateIndex(
            name: "IX_orders_Type_IsRemoved",
            table: "orders",
            columns: OrderTypeIndexColumns);

        migrationBuilder.AddCheckConstraint(
            name: "valid_order_state",
            table: "orders",
            sql: "\"State\" IN ('Registered','Agreed','AwaitingPayment','Paid','AwaitingDelivery','ReadyForPickUp','Completed','Canceled')");

        migrationBuilder.AddCheckConstraint(
            name: "valid_order_type",
            table: "orders",
            sql: "\"Type\" IN ('Stock','Custom')");

        migrationBuilder.AddCheckConstraint(
            name: "valid_category",
            table: "components_categories",
            sql: "\"Id\" IN ('Interior','SteeringWheel','Transmission','Wheel')");

        migrationBuilder.AddCheckConstraint(
            name: "valid_body",
            table: "cars",
            sql: "\"Body\" IN ('Sedan','Hatchback','Wagon','Coupe','Convertible','Liftback','Fastback','Limousine','Minivan','SportUtilityVehicle','CrossoverUtilityVehicle')");

        migrationBuilder.AddCheckConstraint(
            name: "valid_fuel",
            table: "cars",
            sql: "\"Fuel\" IN ('Petrol','Diesel','Electric')");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(
            name: "valid_seasonality",
            table: "wheels");

        migrationBuilder.DropCheckConstraint(
            name: "valid_user_role",
            table: "users");

        migrationBuilder.DropCheckConstraint(
            name: "valid_energy_transfer",
            table: "transmissions");

        migrationBuilder.DropCheckConstraint(
            name: "valid_gearbox",
            table: "transmissions");

        migrationBuilder.DropCheckConstraint(
            name: "valid_wheel_drive",
            table: "transmissions");

        migrationBuilder.DropIndex(
            name: "IX_orders_Type_IsRemoved",
            table: "orders");

        migrationBuilder.DropCheckConstraint(
            name: "valid_order_state",
            table: "orders");

        migrationBuilder.DropCheckConstraint(
            name: "valid_order_type",
            table: "orders");

        migrationBuilder.DropCheckConstraint(
            name: "valid_category",
            table: "components_categories");

        migrationBuilder.DropCheckConstraint(
            name: "valid_body",
            table: "cars");

        migrationBuilder.DropCheckConstraint(
            name: "valid_fuel",
            table: "cars");
    }
}
