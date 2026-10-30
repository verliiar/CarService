using CarService.Domain;

namespace CarService.Application;

public sealed class TestDriveService(IRepository<TestDriveBooking> bookings, IRepository<TestDriveCar> fleet, CarCatalog cars, UserService users, IUnitOfWork unit, TimeProvider clock)
{
    private static BookingDto Map(TestDriveBooking b) => new(b.Id, b.BookerId, b.BookingDateTime, CarSnapshot.From(b.Car));

    public async Task<BookingDto> SaveAsync(Guid userId, Guid? id, BookingRequest request, CancellationToken ct)
    {
        var role = await users.RequireAsync(userId, ct, UserRole.Customer, UserRole.SystemAdmin);
        var date = TestDriveBooking.ValidateDate(request.Date, clock.GetUtcNow());
        var car = await cars.FindAsync(request.CarBrand, request.CarModel, ct);
        if (!await fleet.AnyAsync(f => f.CarId == car.Id, ct))
        {
            throw new BusinessException(FailureKind.NotFound, "Car is not available for test drives.");
        }

        var booking = id == null ? new TestDriveBooking { BookerId = userId } : Rules.Found((await bookings.ListAsync(q => q.Where(b => b.Id == id && !b.IsRemoved), ct)).SingleOrDefault());
        EnsureAccess(booking, userId, role);
        if (await bookings.AnyAsync(b => !b.IsRemoved && b.Id != booking.Id && b.CarId == car.Id && b.BookingDateTime == date, ct))
        {
            throw new BusinessException(FailureKind.Conflict, "Car is already booked at this time.");
        }

        booking.CarId = car.Id;
        booking.Car = car;
        booking.BookingDateTime = date;
        if (id == null)
        {
            bookings.Add(booking);
        }

        await unit.SaveAsync(ct);
        return Map(booking);
    }

    public async Task DeleteAsync(Guid userId, Guid id, CancellationToken ct)
    {
        var role = await users.RequireAsync(userId, ct, UserRole.Customer, UserRole.SystemAdmin);
        var booking = Rules.Found((await bookings.ListAsync(q => q.Where(b => b.Id == id && !b.IsRemoved), ct)).SingleOrDefault());
        EnsureAccess(booking, userId, role);
        booking.IsRemoved = true;
        await unit.SaveAsync(ct);
    }

    private static void EnsureAccess(TestDriveBooking booking, Guid userId, UserRole role)
    {
        if (role != UserRole.SystemAdmin && booking.BookerId != userId)
        {
            throw new BusinessException(FailureKind.Forbidden, "No permission to access this booking.");
        }
    }

    public async Task FleetAsync(Guid userId, string brand, string model, bool remove, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, UserRole.Manager, UserRole.SystemAdmin);
        var car = await cars.FindAsync(brand, model, ct);
        var entry = (await fleet.ListAsync(q => q.Where(f => f.CarId == car.Id), ct)).SingleOrDefault();
        if (remove)
        {
            fleet.Remove(Rules.Found(entry));
        }
        else if (entry == null)
        {
            fleet.Add(new TestDriveCar { CarId = car.Id });
        }

        await unit.SaveAsync(ct);
    }

    public async Task<List<BookingDto>> ListAsync(Guid userId, CancellationToken ct)
    {
        var role = await users.RequireAsync(userId, ct, UserRole.Customer, UserRole.Manager, UserRole.SystemAdmin);
        return (await bookings.ListAsync(q => q.Where(b => !b.IsRemoved && (role != UserRole.Customer || b.BookerId == userId)).OrderBy(b => b.BookingDateTime), ct, false)).Select(Map).ToList();
    }
}
