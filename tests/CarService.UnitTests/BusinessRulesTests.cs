using CarService.Application;
using CarService.Domain;
using Xunit;

namespace CarService.UnitTests;

public sealed class BusinessRulesTests
{
    private static Car MakeCar() => new() { Brand = "BMW", Model = "320i", Price = 100m, Body = BodyType.Sedan, Fuel = FuelType.Petrol, EnginePower = 184, EngineDisplacement = 2, Color = "Black", IsAvailable = true };

    [Theory]
    [InlineData(-1)]
    [InlineData(0.001)]
    public void InvalidMoneyIsRejected(decimal amount) => Assert.Equal(FailureKind.Validation, Assert.Throws<BusinessException>(() => new Money(amount)).Kind);

    [Fact]
    public void MoneyKeepsDecimalPrecision() => Assert.Equal(0.3m, new Money(0.1m + 0.2m).Value);

    [Fact]
    public void ConfigurationAddsOnlyReplacementCostsAndDoesNotMutateCatalog()
    {
        var car = MakeCar();
        var standard = new Wheel { Name = "Standard", AdditionalCost = 10, SupportedCars = [car] };
        var upgrade = new Wheel { Name = "Sport", AdditionalCost = 50, SupportedCars = [car] };
        car.RequiredDetails = [standard];
        var custom = Configurator.Configure(car, [upgrade]);
        Assert.Equal(150, custom.Price);
        Assert.Equal("Sport", custom.RequiredDetails.Single().Name);
        Assert.Equal(100, Configurator.Configure(car, [standard]).Price);
        Assert.Same(standard, car.RequiredDetails.Single());
        Assert.Equal(100, car.Price);
    }

    [Fact]
    public void ConfigurationRejectsMissingDuplicateUnsupportedAndRemovedComponents()
    {
        var car = MakeCar();
        var standard = new Wheel { Name = "Standard", SupportedCars = [car] };
        car.RequiredDetails = [standard];
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, []));
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, [standard, standard]));
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, [new Interior { SupportedCars = [car] }]));
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, [new Wheel()]));
        standard.IsRemoved = true;
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, [standard]));
    }

    [Fact]
    public void ConfigurationRejectsTwoSelectionsFromTheSameCategory()
    {
        var car = MakeCar();
        var wheel = new Wheel { SupportedCars = [car] };
        car.RequiredDetails = [wheel, new Interior()];
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, [wheel, new Wheel { SupportedCars = [car] }]));
    }

    [Theory]
    [InlineData(OrderType.Stock)]
    [InlineData(OrderType.Custom)]
    public void OrdersFollowFullLifecycle(OrderType type)
    {
        var order = new Order { Type = type, Configuration = CarSnapshot.From(MakeCar()) };
        var states = type == OrderType.Stock ? new[] { OrderState.Agreed, OrderState.AwaitingPayment, OrderState.Paid, OrderState.ReadyForPickUp, OrderState.Completed }
            : [OrderState.Agreed, OrderState.AwaitingPayment, OrderState.Paid, OrderState.AwaitingDelivery, OrderState.ReadyForPickUp, OrderState.Completed];
        foreach (var state in states)
        {
            order.Advance(UserRole.SystemAdmin);
            Assert.Equal(state, order.State);
        }
        Assert.Throws<BusinessException>(() => order.Advance(UserRole.SystemAdmin));
        Assert.Throws<BusinessException>(() => order.Cancel(UserRole.Customer));
    }

    public static IEnumerable<object[]> TransitionCases()
    {
        foreach (var type in Enum.GetValues<OrderType>())
        {
            foreach (var role in Enum.GetValues<UserRole>())
            {
                yield return [type, OrderState.Registered, role, type == OrderType.Stock ? role is UserRole.Manager or UserRole.SystemAdmin : role is UserRole.WarehouseAdmin or UserRole.SystemAdmin];
                foreach (var state in new[] { OrderState.Agreed, OrderState.AwaitingPayment, OrderState.Paid })
                {
                    yield return [type, state, role, role is UserRole.Manager or UserRole.SystemAdmin];
                }

                yield return [type, OrderState.AwaitingDelivery, role, role is UserRole.WarehouseAdmin or UserRole.SystemAdmin];
                yield return [type, OrderState.ReadyForPickUp, role, role != UserRole.WarehouseAdmin];
            }
        }
    }

    [Theory, MemberData(nameof(TransitionCases))]
    public void TransitionPermissionsMatchSource(OrderType type, OrderState state, UserRole role, bool allowed)
    {
        var order = new Order { Type = type, State = state, Configuration = CarSnapshot.From(MakeCar()) };
        if (allowed)
        {
            order.Advance(role);
            var expected = state switch
            {
                OrderState.Registered => OrderState.Agreed,
                OrderState.Agreed => OrderState.AwaitingPayment,
                OrderState.AwaitingPayment => OrderState.Paid,
                OrderState.Paid => type == OrderType.Stock ? OrderState.ReadyForPickUp : OrderState.AwaitingDelivery,
                OrderState.AwaitingDelivery => OrderState.ReadyForPickUp,
                _ => OrderState.Completed
            };
            Assert.Equal(expected, order.State);
        }
        else
        {
            Assert.Equal(FailureKind.Forbidden, Assert.Throws<BusinessException>(() => order.Advance(role)).Kind);
            Assert.Equal(state, order.State);
        }
    }

    public static IEnumerable<object[]> CancellationCases()
    {
        foreach (var type in Enum.GetValues<OrderType>())
        {
            foreach (var state in Enum.GetValues<OrderState>())
            {
                foreach (var role in Enum.GetValues<UserRole>())
                {
                    var allowed = state switch
                    {
                        OrderState.Registered => type == OrderType.Stock ? role != UserRole.WarehouseAdmin : role != UserRole.Manager,
                        OrderState.Agreed => role == UserRole.Customer,
                        OrderState.AwaitingPayment => role != UserRole.WarehouseAdmin,
                        _ => false
                    };
                    yield return [type, state, role, allowed];
                }
            }
        }
    }

    [Theory, MemberData(nameof(CancellationCases))]
    public void CancellationRulesMatchSource(OrderType type, OrderState state, UserRole role, bool allowed)
    {
        var order = new Order { Type = type, State = state, Configuration = CarSnapshot.From(MakeCar()) };
        if (allowed)
        {
            order.Cancel(role);
            Assert.Equal(OrderState.Canceled, order.State);
        }
        else
        {
            Assert.Throws<BusinessException>(() => order.Cancel(role));
            Assert.Equal(state, order.State);
        }
    }

    [Fact]
    public void FiltersUseModelPowerAndDisplacementInsteadOfPrice()
    {
        var car = MakeCar();
        var filter = new CarFilter { Brand = "BMW", Model = "320i", EnginePowerLowerLimit = 180, EnginePowerUpperLimit = 200, EngineDisplacementEquals = 2 };
        Assert.Single(filter.Apply(new[] { car }.AsQueryable()));
        Assert.Empty((filter with
        {
            Model = "330i"
        }).Apply(new[] { car }.AsQueryable()));
    }

    [Fact]
    public void RangeBoundsAreExclusiveWithoutEquality()
    {
        var car = MakeCar();
        Assert.Empty(new CarFilter { PriceLowerLimit = 100 }.Apply(new[] { car }.AsQueryable()));
        Assert.Single(new CarFilter { PriceLowerLimit = 100, PriceEquals = 100 }.Apply(new[] { car }.AsQueryable()));
        Assert.Throws<BusinessException>(() => new CarFilter { PriceLowerLimit = 200, PriceUpperLimit = 100 }.Apply(new[] { car }.AsQueryable()));
        Assert.Throws<BusinessException>(() => new CarFilter { Model = "320i" }.Apply(new[] { car }.AsQueryable()));
    }

    [Fact]
    public void BookingDateMustBeFutureAndIsNormalizedToUtc()
    {
        var now = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<BusinessException>(() => TestDriveBooking.ValidateDate(now, now));
        Assert.Throws<BusinessException>(() => TestDriveBooking.ValidateDate(now.AddDays(-1), now));
        Assert.Equal(TimeSpan.Zero, TestDriveBooking.ValidateDate(now.AddHours(1).ToOffset(TimeSpan.FromHours(3)), now).Offset);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("customer")]
    [InlineData("Invalid")]
    public void InvalidRolesAreRejected(string role) => Assert.Throws<BusinessException>(() => Rules.Parse<UserRole>(role));

    [Fact]
    public void MoneyRejectsValuesOutsideDatabasePrecision() => Assert.Throws<BusinessException>(() => new Money(10000000000000000m));

    [Fact]
    public void ConfigurationRejectsRemovedCarAndEmptyOrRemovedBaseConfiguration()
    {
        var car = MakeCar();
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, []));
        var wheel = new Wheel { SupportedCars = [car] };
        car.RequiredDetails = [wheel];
        car.IsRemoved = true;
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, [wheel]));
        car.IsRemoved = false;
        wheel.IsRemoved = true;
        Assert.Throws<BusinessException>(() => Configurator.Configure(car, [new Wheel { SupportedCars = [car] }]));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0.00001)]
    [InlineData(100000000)]
    public void MeasurementsRejectSilentDatabaseRounding(decimal value) => Assert.Throws<BusinessException>(() => Rules.Measurement(value));

    [Fact]
    public void InvalidEnumCannotAcquireTransitionPermissions()
    {
        var order = new Order { State = OrderState.ReadyForPickUp, Configuration = CarSnapshot.From(MakeCar()) };
        Assert.Throws<BusinessException>(() => order.Advance((UserRole)999));
        Assert.Equal(OrderState.ReadyForPickUp, order.State);
        order.State = OrderState.Registered;
        Assert.Throws<BusinessException>(() => order.Cancel((UserRole)999));
        Assert.Equal(OrderState.Registered, order.State);
    }
}
