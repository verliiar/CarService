namespace CarService.Domain;

public enum OrderState
{
    Registered,
    Agreed,
    AwaitingPayment,
    Paid,
    AwaitingDelivery,
    ReadyForPickUp,
    Completed,
    Canceled,
}
