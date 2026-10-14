namespace GasMonitoringApi.Dtos;

public record LoginResponse(string AccessToken, string Login, string Role, DateTimeOffset ExpiresAt);
