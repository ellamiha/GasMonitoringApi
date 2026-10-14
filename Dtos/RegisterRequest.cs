using System.ComponentModel.DataAnnotations;

namespace GasMonitoringApi.Dtos;

public class RegisterRequest
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.-]{3,40}$", ErrorMessage = "Логин: 3–40 латинских букв, цифр, точек, дефисов или подчёркиваний.")]
    public string Login { get; set; } = "";

    [Required]
    [StringLength(128, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*[0-9]).+$", ErrorMessage = "Пароль должен содержать латинскую букву и цифру.")]
    public string Password { get; set; } = "";
}
