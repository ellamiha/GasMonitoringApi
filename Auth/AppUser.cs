namespace GasMonitoringApi.Auth;

public class AppUser
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string NormalizedLogin { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = UserRoles.User;
}
