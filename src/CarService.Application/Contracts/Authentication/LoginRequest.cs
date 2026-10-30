using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarService.Application;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record LoginRequest(
    [Required, EmailAddress, MaxLength(254)] string Email,
    [Required, MaxLength(128)] string Password);
