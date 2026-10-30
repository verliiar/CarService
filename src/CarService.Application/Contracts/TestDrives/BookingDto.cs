using CarService.Domain;

namespace CarService.Application;

public record BookingDto(Guid BookingId, Guid BookerId, DateTimeOffset BookingDateTime, CarSnapshot Car);
