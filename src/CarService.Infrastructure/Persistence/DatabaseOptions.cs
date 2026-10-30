using System.ComponentModel.DataAnnotations;

namespace CarService.Infrastructure;

public sealed class DatabaseOptions
{
    [Required] public string ConnectionString { get; set; } = "";

    public bool MigrateOnStartup { get; set; } = true;
}
