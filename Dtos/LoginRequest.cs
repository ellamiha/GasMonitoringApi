using System.ComponentModel.DataAnnotations;

namespace GasMonitoringApi.Dtos;

public class LoginRequest
{
    [Required]
    [StringLength(40)]
    public string Login { get; set; } = "";

    [Required]
    [StringLength(128)]
    public string Password { get; set; } = "";
}
