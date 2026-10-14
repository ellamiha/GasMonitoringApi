using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace GasMonitoringApi.Auth;

public static class AuthSettings
{
    public const string Issuer = "GasMonitoringApi";
    public const string Audience = "GasMonitoringClient";

    public static SymmetricSecurityKey SigningKey { get; } =
        new(RandomNumberGenerator.GetBytes(32));
}