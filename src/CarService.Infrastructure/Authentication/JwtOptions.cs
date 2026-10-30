using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarService.Infrastructure;

public sealed class JwtOptions
{
    [Required] public string Issuer { get; set; } = "";

    [Required] public string Audience { get; set; } = "";
    [Required, MinLength(32)] public string SigningKey { get; set; } = "";

    [Range(1, 60)] public int AccessTokenMinutes { get; set; } = 15;
    public byte[] GetSigningKey() => Encoding.UTF8.GetBytes(SigningKey);
}
