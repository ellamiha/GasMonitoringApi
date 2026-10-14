using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GasMonitoringApi.Auth;
using GasMonitoringApi.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace GasMonitoringApi.Services;

public class TokenService
{
    private readonly JwtSettings settings;

    public TokenService(JwtSettings settings)
    {
        this.settings = settings;
    }

    public LoginResponse CreateToken(AppUser user)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddHours(2);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("name", user.Login),
            new Claim("role", user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(settings.SigningKey, SecurityAlgorithms.HmacSha256));
        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token),
            user.Login, user.Role, expiresAt);
    }
}
