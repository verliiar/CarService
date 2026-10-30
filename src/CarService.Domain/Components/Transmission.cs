namespace CarService.Domain;

public sealed class Transmission : Component
{
    public Transmission() => Category = ComponentCategory.Transmission;

    public GearboxType Gearbox
    {
        get; set;
    }

    public WheelDriveType WheelDrive
    {
        get; set;
    }

    public EnergyTransferType EnergyTransfer
    {
        get; set;
    }
}
