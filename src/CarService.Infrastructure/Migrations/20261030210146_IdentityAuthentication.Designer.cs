using System;
using CarService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarService.Infrastructure.Migrations
{
    [DbContext(typeof(DealershipDbContext))]
    [Migration("20261030210146_IdentityAuthentication")]
    partial class IdentityAuthentication
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CarService.Domain.Car", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("EngineDisplacement")
                        .HasPrecision(14, 4)
                        .HasColumnType("numeric(14,4)");

                    b.Property<decimal>("EnginePower")
                        .HasPrecision(14, 4)
                        .HasColumnType("numeric(14,4)");

                    b.Property<string>("Fuel")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.HasIndex("IsAvailable");

                    b.HasIndex("Brand", "Model")
                        .IsUnique();

                    b.ToTable("cars", null, t =>
                        {
                            t.HasCheckConstraint("car_engine", "\"EnginePower\" >= 0 AND \"EngineDisplacement\" >= 0");

                            t.HasCheckConstraint("car_price", "\"Price\" >= 0");

                            t.HasCheckConstraint("valid_body", "\"Body\" IN ('Sedan','Hatchback','Wagon','Coupe','Convertible','Liftback','Fastback','Limousine','Minivan','SportUtilityVehicle','CrossoverUtilityVehicle')");

                            t.HasCheckConstraint("valid_fuel", "\"Fuel\" IN ('Petrol','Diesel','Electric')");
                        });

                    b.HasData(
                        new
                        {
                            Id = new Guid("11111111-1111-1111-1111-111111111111"),
                            Body = "Sedan",
                            Brand = "BMW",
                            Color = "Black",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            EngineDisplacement = 2.0m,
                            EnginePower = 184m,
                            Fuel = "Petrol",
                            IsAvailable = true,
                            IsRemoved = false,
                            Model = "320i",
                            Price = 3500000m,
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        },
                        new
                        {
                            Id = new Guid("22222222-2222-2222-2222-222222222222"),
                            Body = "Sedan",
                            Brand = "BMW",
                            Color = "White",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            EngineDisplacement = 2.0m,
                            EnginePower = 258m,
                            Fuel = "Petrol",
                            IsAvailable = true,
                            IsRemoved = false,
                            Model = "330i",
                            Price = 4200000m,
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        },
                        new
                        {
                            Id = new Guid("33333333-3333-3333-3333-333333333333"),
                            Body = "Sedan",
                            Brand = "Audi",
                            Color = "Grey",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            EngineDisplacement = 2.0m,
                            EnginePower = 190m,
                            Fuel = "Diesel",
                            IsAvailable = false,
                            IsRemoved = false,
                            Model = "A4",
                            Price = 3000000m,
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        },
                        new
                        {
                            Id = new Guid("44444444-4444-4444-4444-444444444444"),
                            Body = "Sedan",
                            Brand = "Toyota",
                            Color = "Red",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            EngineDisplacement = 2.5m,
                            EnginePower = 203m,
                            Fuel = "Petrol",
                            IsAvailable = true,
                            IsRemoved = false,
                            Model = "Camry",
                            Price = 2700000m,
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        },
                        new
                        {
                            Id = new Guid("55555555-5555-5555-5555-555555555555"),
                            Body = "Wagon",
                            Brand = "Volkswagen",
                            Color = "Black",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            EngineDisplacement = 2.0m,
                            EnginePower = 190m,
                            Fuel = "Diesel",
                            IsAvailable = true,
                            IsRemoved = false,
                            Model = "Passat",
                            Price = 3200000m,
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        });
                });

            modelBuilder.Entity("CarService.Domain.Component", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("AdditionalCost")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.HasIndex("Category", "Name")
                        .IsUnique();

                    b.ToTable("components", null, t =>
                        {
                            t.HasCheckConstraint("component_price", "\"AdditionalCost\" >= 0");
                        });

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("CarService.Domain.ComponentCategoryEntry", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("components_categories", null, t =>
                        {
                            t.HasCheckConstraint("valid_category", "\"Id\" IN ('Interior','SteeringWheel','Transmission','Wheel')");
                        });

                    b.HasData(
                        new
                        {
                            Id = "Interior",
                            Name = "Interior"
                        },
                        new
                        {
                            Id = "SteeringWheel",
                            Name = "SteeringWheel"
                        },
                        new
                        {
                            Id = "Transmission",
                            Name = "Transmission"
                        },
                        new
                        {
                            Id = "Wheel",
                            Name = "Wheel"
                        });
                });

            modelBuilder.Entity("CarService.Domain.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uuid");

                    b.Property<string>("Configuration")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ManagerId")
                        .HasColumnType("uuid");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ManagerId");

                    b.HasIndex("Type", "IsRemoved");

                    b.ToTable("orders", null, t =>
                        {
                            t.HasCheckConstraint("valid_order_state", "\"State\" IN ('Registered','Agreed','AwaitingPayment','Paid','AwaitingDelivery','ReadyForPickUp','Completed','Canceled')");

                            t.HasCheckConstraint("valid_order_type", "\"Type\" IN ('Stock','Custom')");
                        });

                    b.HasData(
                        new
                        {
                            Id = new Guid("11111111-1111-1111-1111-111111111111"),
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            Configuration = "{\"Brand\":\"BMW\",\"Model\":\"320i\",\"Price\":3500000,\"RequiredDetails\":[{\"CategoryName\":\"Interior\",\"Name\":\"\\u0422\\u043A\\u0430\\u043D\\u0435\\u0432\\u044B\\u0439 Graphite\"},{\"CategoryName\":\"SteeringWheel\",\"Name\":\"\\u0421\\u043F\\u043E\\u0440\\u0442\\u0438\\u0432\\u043D\\u044B\\u0439 \\u043A\\u043E\\u0436\\u0430\\u043D\\u044B\\u0439 (Standard)\"},{\"CategoryName\":\"Transmission\",\"Name\":\"\\u0410\\u0432\\u0442\\u043E\\u043C\\u0430\\u0442\\u0438\\u0447\\u0435\\u0441\\u043A\\u0430\\u044F 8AT\"},{\"CategoryName\":\"Wheel\",\"Name\":\"17\\u2019\\u2019 Standard\"}],\"Body\":\"Sedan\",\"Engine\":{\"Fuel\":\"Petrol\",\"Power\":184,\"Displacement\":2.0},\"Color\":\"Black\",\"IsAvailable\":true}",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            CustomerId = new Guid("11111111-1111-1111-1111-111111111111"),
                            IsRemoved = false,
                            ManagerId = new Guid("33333333-3333-3333-3333-333333333333"),
                            State = "Registered",
                            Type = "Stock",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        },
                        new
                        {
                            Id = new Guid("22222222-2222-2222-2222-222222222222"),
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            Configuration = "{\"Brand\":\"Audi\",\"Model\":\"A4\",\"Price\":3000000,\"RequiredDetails\":[{\"CategoryName\":\"Interior\",\"Name\":\"\\u0422\\u043A\\u0430\\u043D\\u0435\\u0432\\u044B\\u0439 Graphite\"},{\"CategoryName\":\"SteeringWheel\",\"Name\":\"M-Sport \\u0441 \\u043F\\u043E\\u0434\\u043E\\u0433\\u0440\\u0435\\u0432\\u043E\\u043C\"},{\"CategoryName\":\"Transmission\",\"Name\":\"\\u041C\\u0435\\u0445\\u0430\\u043D\\u0438\\u0447\\u0435\\u0441\\u043A\\u0430\\u044F 6MT\"},{\"CategoryName\":\"Wheel\",\"Name\":\"18\\u2019\\u2019 Aero\"}],\"Body\":\"Sedan\",\"Engine\":{\"Fuel\":\"Diesel\",\"Power\":190,\"Displacement\":2.0},\"Color\":\"Grey\",\"IsAvailable\":false}",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            CustomerId = new Guid("22222222-2222-2222-2222-222222222222"),
                            IsRemoved = false,
                            ManagerId = new Guid("33333333-3333-3333-3333-333333333333"),
                            State = "AwaitingPayment",
                            Type = "Custom",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        },
                        new
                        {
                            Id = new Guid("33333333-3333-3333-3333-333333333333"),
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            Configuration = "{\"Brand\":\"Toyota\",\"Model\":\"Camry\",\"Price\":2700000,\"RequiredDetails\":[{\"CategoryName\":\"Interior\",\"Name\":\"\\u0422\\u043A\\u0430\\u043D\\u0435\\u0432\\u044B\\u0439 Graphite\"},{\"CategoryName\":\"SteeringWheel\",\"Name\":\"\\u0421\\u043F\\u043E\\u0440\\u0442\\u0438\\u0432\\u043D\\u044B\\u0439 \\u043A\\u043E\\u0436\\u0430\\u043D\\u044B\\u0439 (Standard)\"},{\"CategoryName\":\"Transmission\",\"Name\":\"\\u0410\\u0432\\u0442\\u043E\\u043C\\u0430\\u0442\\u0438\\u0447\\u0435\\u0441\\u043A\\u0430\\u044F 8AT\"},{\"CategoryName\":\"Wheel\",\"Name\":\"17\\u2019\\u2019 Standard\"}],\"Body\":\"Sedan\",\"Engine\":{\"Fuel\":\"Petrol\",\"Power\":203,\"Displacement\":2.5},\"Color\":\"Red\",\"IsAvailable\":true}",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            CustomerId = new Guid("11111111-1111-1111-1111-111111111111"),
                            IsRemoved = false,
                            ManagerId = new Guid("33333333-3333-3333-3333-333333333333"),
                            State = "Completed",
                            Type = "Stock",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        });
                });

            modelBuilder.Entity("CarService.Domain.TestDriveBooking", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BookerId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("BookingDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.HasIndex("BookerId");

                    b.HasIndex("CarId", "BookingDateTime")
                        .IsUnique()
                        .HasFilter("NOT \"IsRemoved\"");

                    b.ToTable("test_drive_bookings", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("11111111-1111-1111-1111-111111111111"),
                            BookerId = new Guid("11111111-1111-1111-1111-111111111111"),
                            BookingDateTime = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        },
                        new
                        {
                            Id = new Guid("22222222-2222-2222-2222-222222222222"),
                            BookerId = new Guid("22222222-2222-2222-2222-222222222222"),
                            BookingDateTime = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u
                        });
                });

            modelBuilder.Entity("CarService.Domain.TestDriveCar", b =>
                {
                    b.Property<Guid>("CarId")
                        .HasColumnType("uuid");

                    b.HasKey("CarId");

                    b.ToTable("test_drive_cars", (string)null);

                    b.HasData(
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444")
                        });
                });

            modelBuilder.Entity("CarService.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("users", null, t =>
                        {
                            t.HasCheckConstraint("valid_user_role", "\"Role\" IN ('Manager','WarehouseAdmin','Customer','SystemAdmin')");
                        });

                    b.HasData(
                        new
                        {
                            Id = new Guid("11111111-1111-1111-1111-111111111111"),
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Masha",
                            Role = "Customer",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0))
                        },
                        new
                        {
                            Id = new Guid("22222222-2222-2222-2222-222222222222"),
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Gleb",
                            Role = "Customer",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0))
                        },
                        new
                        {
                            Id = new Guid("33333333-3333-3333-3333-333333333333"),
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Sasha",
                            Role = "Manager",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0))
                        },
                        new
                        {
                            Id = new Guid("44444444-4444-4444-4444-444444444444"),
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Daniil",
                            Role = "WarehouseAdmin",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0))
                        },
                        new
                        {
                            Id = new Guid("55555555-5555-5555-5555-555555555555"),
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Nahid",
                            Role = "SystemAdmin",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0))
                        });
                });

            modelBuilder.Entity("CarService.Infrastructure.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .IsUnique()
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("account_claims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("account_logins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("account_tokens", (string)null);
                });

            modelBuilder.Entity("car_components", b =>
                {
                    b.Property<Guid>("CarId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ComponentId")
                        .HasColumnType("uuid");

                    b.HasKey("CarId", "ComponentId");

                    b.HasIndex("ComponentId");

                    b.ToTable("car_components");

                    b.HasData(
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("88888888-8888-8888-8888-888888888888")
                        },
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("66666666-6666-6666-6666-666666666666")
                        },
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("11111111-1111-1111-1111-111111111111")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("99999999-9999-9999-9999-999999999999")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("66666666-6666-6666-6666-666666666666")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("11111111-1111-1111-1111-111111111111")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("88888888-8888-8888-8888-888888888888")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("77777777-7777-7777-7777-777777777777")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("55555555-5555-5555-5555-555555555555")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("33333333-3333-3333-3333-333333333333")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            ComponentId = new Guid("88888888-8888-8888-8888-888888888888")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            ComponentId = new Guid("66666666-6666-6666-6666-666666666666")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            ComponentId = new Guid("11111111-1111-1111-1111-111111111111")
                        },
                        new
                        {
                            CarId = new Guid("55555555-5555-5555-5555-555555555555"),
                            ComponentId = new Guid("99999999-9999-9999-9999-999999999999")
                        },
                        new
                        {
                            CarId = new Guid("55555555-5555-5555-5555-555555555555"),
                            ComponentId = new Guid("77777777-7777-7777-7777-777777777777")
                        },
                        new
                        {
                            CarId = new Guid("55555555-5555-5555-5555-555555555555"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("55555555-5555-5555-5555-555555555555"),
                            ComponentId = new Guid("22222222-2222-2222-2222-222222222222")
                        });
                });

            modelBuilder.Entity("compatibilities", b =>
                {
                    b.Property<Guid>("CarId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ComponentId")
                        .HasColumnType("uuid");

                    b.HasKey("CarId", "ComponentId");

                    b.HasIndex("ComponentId");

                    b.ToTable("compatibilities");

                    b.HasData(
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("11111111-1111-1111-1111-111111111111")
                        },
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("22222222-2222-2222-2222-222222222222")
                        },
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("55555555-5555-5555-5555-555555555555")
                        },
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("66666666-6666-6666-6666-666666666666")
                        },
                        new
                        {
                            CarId = new Guid("11111111-1111-1111-1111-111111111111"),
                            ComponentId = new Guid("88888888-8888-8888-8888-888888888888")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("11111111-1111-1111-1111-111111111111")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("22222222-2222-2222-2222-222222222222")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("55555555-5555-5555-5555-555555555555")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("66666666-6666-6666-6666-666666666666")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("77777777-7777-7777-7777-777777777777")
                        },
                        new
                        {
                            CarId = new Guid("22222222-2222-2222-2222-222222222222"),
                            ComponentId = new Guid("99999999-9999-9999-9999-999999999999")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("33333333-3333-3333-3333-333333333333")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("55555555-5555-5555-5555-555555555555")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("77777777-7777-7777-7777-777777777777")
                        },
                        new
                        {
                            CarId = new Guid("33333333-3333-3333-3333-333333333333"),
                            ComponentId = new Guid("88888888-8888-8888-8888-888888888888")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            ComponentId = new Guid("11111111-1111-1111-1111-111111111111")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            ComponentId = new Guid("66666666-6666-6666-6666-666666666666")
                        },
                        new
                        {
                            CarId = new Guid("44444444-4444-4444-4444-444444444444"),
                            ComponentId = new Guid("88888888-8888-8888-8888-888888888888")
                        },
                        new
                        {
                            CarId = new Guid("55555555-5555-5555-5555-555555555555"),
                            ComponentId = new Guid("22222222-2222-2222-2222-222222222222")
                        },
                        new
                        {
                            CarId = new Guid("55555555-5555-5555-5555-555555555555"),
                            ComponentId = new Guid("44444444-4444-4444-4444-444444444444")
                        },
                        new
                        {
                            CarId = new Guid("55555555-5555-5555-5555-555555555555"),
                            ComponentId = new Guid("77777777-7777-7777-7777-777777777777")
                        },
                        new
                        {
                            CarId = new Guid("55555555-5555-5555-5555-555555555555"),
                            ComponentId = new Guid("99999999-9999-9999-9999-999999999999")
                        });
                });

            modelBuilder.Entity("CarService.Domain.Interior", b =>
                {
                    b.HasBaseType("CarService.Domain.Component");

                    b.Property<string>("Material")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.ToTable("interiors", null, t =>
                        {
                            t.HasCheckConstraint("component_price", "\"AdditionalCost\" >= 0");
                        });

                    b.HasData(
                        new
                        {
                            Id = new Guid("88888888-8888-8888-8888-888888888888"),
                            AdditionalCost = 10000m,
                            Category = "Interior",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Тканевый Graphite",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            Material = "Fabric"
                        },
                        new
                        {
                            Id = new Guid("99999999-9999-9999-9999-999999999999"),
                            AdditionalCost = 110000m,
                            Category = "Interior",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Кожаный Dakota",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            Material = "Leather"
                        });
                });

            modelBuilder.Entity("CarService.Domain.SteeringWheel", b =>
                {
                    b.HasBaseType("CarService.Domain.Component");

                    b.Property<string>("Material")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Shape")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<decimal>("Size")
                        .HasPrecision(14, 4)
                        .HasColumnType("numeric(14,4)");

                    b.ToTable("steering_wheels", null, t =>
                        {
                            t.HasCheckConstraint("component_price", "\"AdditionalCost\" >= 0");

                            t.HasCheckConstraint("steering_size", "\"Size\" > 0");
                        });

                    b.HasData(
                        new
                        {
                            Id = new Guid("66666666-6666-6666-6666-666666666666"),
                            AdditionalCost = 5000m,
                            Category = "SteeringWheel",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Спортивный кожаный (Standard)",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            Material = "Leather",
                            Shape = "Round",
                            Size = 38m
                        },
                        new
                        {
                            Id = new Guid("77777777-7777-7777-7777-777777777777"),
                            AdditionalCost = 25000m,
                            Category = "SteeringWheel",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "M-Sport с подогревом",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            Material = "Leather",
                            Shape = "Round",
                            Size = 36m
                        });
                });

            modelBuilder.Entity("CarService.Domain.Transmission", b =>
                {
                    b.HasBaseType("CarService.Domain.Component");

                    b.Property<string>("EnergyTransfer")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Gearbox")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("WheelDrive")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.ToTable("transmissions", null, t =>
                        {
                            t.HasCheckConstraint("component_price", "\"AdditionalCost\" >= 0");

                            t.HasCheckConstraint("valid_energy_transfer", "\"EnergyTransfer\" IN ('Mechanical','Hydromechanical','Hydrostatic')");

                            t.HasCheckConstraint("valid_gearbox", "\"Gearbox\" IN ('Manual','Automatic')");

                            t.HasCheckConstraint("valid_wheel_drive", "\"WheelDrive\" IN ('Front','Rear','All')");
                        });

                    b.HasData(
                        new
                        {
                            Id = new Guid("44444444-4444-4444-4444-444444444444"),
                            AdditionalCost = 5000m,
                            Category = "Transmission",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Автоматическая 8AT",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            EnergyTransfer = "Mechanical",
                            Gearbox = "Automatic",
                            WheelDrive = "All"
                        },
                        new
                        {
                            Id = new Guid("55555555-5555-5555-5555-555555555555"),
                            AdditionalCost = 30000m,
                            Category = "Transmission",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "Механическая 6MT",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            EnergyTransfer = "Mechanical",
                            Gearbox = "Manual",
                            WheelDrive = "All"
                        });
                });

            modelBuilder.Entity("CarService.Domain.Wheel", b =>
                {
                    b.HasBaseType("CarService.Domain.Component");

                    b.Property<decimal>("Diameter")
                        .HasPrecision(14, 4)
                        .HasColumnType("numeric(14,4)");

                    b.Property<int>("LoadIndex")
                        .HasColumnType("integer");

                    b.Property<string>("Seasonality")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.ToTable("wheels", null, t =>
                        {
                            t.HasCheckConstraint("component_price", "\"AdditionalCost\" >= 0");

                            t.HasCheckConstraint("valid_seasonality", "\"Seasonality\" IN ('Summer','Winter','All')");

                            t.HasCheckConstraint("wheel_dimensions", "\"Diameter\" > 0 AND \"LoadIndex\" > 0");
                        });

                    b.HasData(
                        new
                        {
                            Id = new Guid("11111111-1111-1111-1111-111111111111"),
                            AdditionalCost = 7000m,
                            Category = "Wheel",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "17’’ Standard",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            Diameter = 17m,
                            LoadIndex = 91,
                            Seasonality = "All"
                        },
                        new
                        {
                            Id = new Guid("22222222-2222-2222-2222-222222222222"),
                            AdditionalCost = 95000m,
                            Category = "Wheel",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "19’’ M-Sport",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            Diameter = 19m,
                            LoadIndex = 95,
                            Seasonality = "Summer"
                        },
                        new
                        {
                            Id = new Guid("33333333-3333-3333-3333-333333333333"),
                            AdditionalCost = 45000m,
                            Category = "Wheel",
                            CreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsRemoved = false,
                            Name = "18’’ Aero",
                            UpdatedAt = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Version = 0u,
                            Diameter = 18m,
                            LoadIndex = 93,
                            Seasonality = "Winter"
                        });
                });

            modelBuilder.Entity("CarService.Domain.Component", b =>
                {
                    b.HasOne("CarService.Domain.ComponentCategoryEntry", null)
                        .WithMany()
                        .HasForeignKey("Category")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("CarService.Domain.Order", b =>
                {
                    b.HasOne("CarService.Domain.Car", null)
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CarService.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CarService.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("CarService.Domain.TestDriveBooking", b =>
                {
                    b.HasOne("CarService.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("BookerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CarService.Domain.Car", "Car")
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Car");
                });

            modelBuilder.Entity("CarService.Domain.TestDriveCar", b =>
                {
                    b.HasOne("CarService.Domain.Car", null)
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("CarService.Infrastructure.ApplicationUser", b =>
                {
                    b.HasOne("CarService.Domain.User", "Profile")
                        .WithOne()
                        .HasForeignKey("CarService.Infrastructure.ApplicationUser", "Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("CarService.Infrastructure.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("CarService.Infrastructure.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("CarService.Infrastructure.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("car_components", b =>
                {
                    b.HasOne("CarService.Domain.Car", null)
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarService.Domain.Component", null)
                        .WithMany()
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("compatibilities", b =>
                {
                    b.HasOne("CarService.Domain.Car", null)
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarService.Domain.Component", null)
                        .WithMany()
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CarService.Domain.Interior", b =>
                {
                    b.HasOne("CarService.Domain.Component", null)
                        .WithOne()
                        .HasForeignKey("CarService.Domain.Interior", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CarService.Domain.SteeringWheel", b =>
                {
                    b.HasOne("CarService.Domain.Component", null)
                        .WithOne()
                        .HasForeignKey("CarService.Domain.SteeringWheel", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CarService.Domain.Transmission", b =>
                {
                    b.HasOne("CarService.Domain.Component", null)
                        .WithOne()
                        .HasForeignKey("CarService.Domain.Transmission", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CarService.Domain.Wheel", b =>
                {
                    b.HasOne("CarService.Domain.Component", null)
                        .WithOne()
                        .HasForeignKey("CarService.Domain.Wheel", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
