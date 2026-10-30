using System.ComponentModel.DataAnnotations;

namespace CarService.Application;

public sealed record ChangeRoleRequest([Required] string Role);
