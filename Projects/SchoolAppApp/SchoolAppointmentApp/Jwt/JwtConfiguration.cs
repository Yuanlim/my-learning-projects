using System.ComponentModel.DataAnnotations;

namespace SchoolAppointmentApp.Jwt;

public class JwtConfiguration
{
  [Required, MinLength(32)] public string SecretKey { get; init; } = "";
  [Required] public string Issuer { get; init; } = "";
  [Required] public string Audience { get; init; } = "";
  [Range(1, 1440)] public int TokenExpiryInMin { get; init; } = 30;
}
