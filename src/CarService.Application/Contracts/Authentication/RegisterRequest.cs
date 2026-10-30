using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarService.Application;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record RegisterRequest(
    [Required, EmailAddress, MaxLength(254)] string Email,
    [Required, StringLength(128, MinimumLength = 12)] string Password,
    [Required, MaxLength(64)] string Name);
