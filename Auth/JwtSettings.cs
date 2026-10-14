using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace GasMonitoringApi.Auth;

public class JwtSettings
{
    public string Issuer { get; } = "GasMonitoringApi";
    public string Audience { get; } = "GasMonitoringClient";
    public SymmetricSecurityKey SigningKey { get; }

    public JwtSettings(string keyPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(keyPath)!);
        if (!File.Exists(keyPath))
        {
            try
            {
                using var stream = new FileStream(keyPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                stream.Write(RandomNumberGenerator.GetBytes(32));
                if (!OperatingSystem.IsWindows())
                    File.SetUnixFileMode(keyPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            }
            catch (IOException) when (File.Exists(keyPath))
            {
            }
        }
        var key = File.ReadAllBytes(keyPath);
        if (key.Length < 32)
            throw new InvalidOperationException("Ключ JWT должен содержать не менее 32 байт.");
        SigningKey = new SymmetricSecurityKey(key);
    }
}
