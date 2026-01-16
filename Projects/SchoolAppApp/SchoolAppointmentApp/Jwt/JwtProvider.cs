using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace SchoolAppointmentApp.Jwt;

internal sealed class JwtProvider(IOptions<JwtConfiguration> configurations)
{
    private readonly JwtConfiguration _jwt = configurations.Value;

    public string Create(IEnumerable<Claim> claims)
    {
        string secretKey = _jwt.SecretKey;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.TokenExpiryInMin),
            signingCredentials: credentials
        );

        Console.WriteLine(DateTime.UtcNow);
        Console.WriteLine(DateTime.UtcNow.AddMinutes(_jwt.TokenExpiryInMin));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}