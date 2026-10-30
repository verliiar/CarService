namespace CarService.Domain;

public sealed class Order : Entity
{
    public Guid CustomerId
    {
        get; set;
    }

    public Guid ManagerId
    {
        get; set;
    }

    public Guid CarId
    {
        get; set;
    }

    public OrderType Type
    {
        get; set;
    }

    public OrderState State { get; set; } = OrderState.Registered;

    public required CarSnapshot Configuration
    {
        get; set;
    }

    public uint Version
    {
        get; set;
    }

    public void Advance(UserRole role)
    {
        if (!Enum.IsDefined(role) || !Enum.IsDefined(Type))
        {
            throw new BusinessException(FailureKind.Validation, "Invalid order type or role.");
        }

        var allowed = State switch
        {
            OrderState.Registered => Type == OrderType.Stock ? role is UserRole.Manager or UserRole.SystemAdmin : role is UserRole.WarehouseAdmin or UserRole.SystemAdmin,
            OrderState.Agreed or OrderState.AwaitingPayment or OrderState.Paid => role is UserRole.Manager or UserRole.SystemAdmin,
            OrderState.AwaitingDelivery => role is UserRole.WarehouseAdmin or UserRole.SystemAdmin,
            OrderState.ReadyForPickUp => role != UserRole.WarehouseAdmin,
            _ => throw new BusinessException(FailureKind.Validation, "Order is already terminal.")
        };
        if (!allowed)
        {
            throw new BusinessException(FailureKind.Forbidden, "No rights to change the order status.");
        }

        State = State switch
        {
            OrderState.Registered => OrderState.Agreed,
            OrderState.Agreed => OrderState.AwaitingPayment,
            OrderState.AwaitingPayment => OrderState.Paid,
            OrderState.Paid => Type == OrderType.Stock ? OrderState.ReadyForPickUp : OrderState.AwaitingDelivery,
            OrderState.AwaitingDelivery => OrderState.ReadyForPickUp,
            _ => OrderState.Completed
        };
    }

    public void Cancel(UserRole role)
    {
        if (!Enum.IsDefined(role) || !Enum.IsDefined(Type))
        {
            throw new BusinessException(FailureKind.Validation, "Invalid order type or role.");
        }

        var allowed = State switch
        {
            OrderState.Registered => Type == OrderType.Stock ? role != UserRole.WarehouseAdmin : role != UserRole.Manager,
            OrderState.Agreed => role == UserRole.Customer,
            OrderState.AwaitingPayment => role != UserRole.WarehouseAdmin,
            _ => throw new BusinessException(FailureKind.Validation, "Cannot cancel order at this stage.")
        };
        if (!allowed)
        {
            throw new BusinessException(FailureKind.Forbidden, "No rights to cancel the order.");
        }

        State = OrderState.Canceled;
    }
}
