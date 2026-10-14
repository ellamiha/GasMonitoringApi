using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GasMonitoringApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GasMonitoringApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        string role;

        if (request.Login == "admin" &&
            request.Password == "admin123")
        {
            role = "Admin";
        }
        else if (request.Login == "user" &&
                 request.Password == "user123")
        {
            role = "User";
        }
        else
        {
            return Unauthorized("Неверный логин или пароль.");
        }

        var claims = new[]
        {
            new Claim("name", request.Login),
            new Claim("role", role),
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var expiresAt = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: AuthSettings.Issuer,
            audience: AuthSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                AuthSettings.SigningKey,
                SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler()
                .WriteToken(token),
            login = request.Login,
            role,
            expiresAt
        });
    }
}