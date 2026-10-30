using System.ComponentModel.DataAnnotations;

namespace CarService.Application;

public sealed record BookingRequest([Required, MaxLength(64)] string CarBrand, [Required, MaxLength(64)] string CarModel, DateTimeOffset Date);
