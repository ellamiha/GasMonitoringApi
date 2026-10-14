using System.ComponentModel.DataAnnotations;
using System.Text;
using GasMonitoringApi.Dtos;
using GasMonitoringApi.Services;

namespace GasMonitoringApi.Infrastructure;

public static class AdminSetup
{
    public static async Task RunAsync(AuthService authService, bool resetPassword = false)
    {
        Console.Write("Логин администратора: ");
        var login = Console.ReadLine() ?? "";
        Console.Write("Пароль (минимум 8 символов, латинская буква и цифра): ");
        var password = ReadPassword();
        var request = new RegisterRequest { Login = login, Password = password };
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, new ValidationContext(request), errors, true))
        {
            foreach (var error in errors)
                Console.WriteLine(error.ErrorMessage);
            Environment.ExitCode = 1;
            return;
        }
        Console.Write("Повтори пароль: ");
        if (ReadPassword() != password)
        {
            Console.WriteLine("Пароли не совпадают. Изменения не сохранены. Запусти команду ещё раз.");
            Environment.ExitCode = 1;
            return;
        }
        try
        {
            var user = resetPassword
                ? await authService.ResetAdminPasswordAsync(request, CancellationToken.None)
                : await authService.CreateAdminAsync(request, CancellationToken.None);
            Console.WriteLine(resetPassword
                ? $"Пароль администратора {user.Login} изменён. Теперь запусти API обычной командой."
                : $"Администратор {user.Login} создан. Теперь запусти API обычной командой.");
        }
        catch (ServiceException error)
        {
            Console.WriteLine(error.Message);
            Environment.ExitCode = 1;
        }
    }

    private static string ReadPassword()
    {
        if (Console.IsInputRedirected)
            return Console.ReadLine() ?? "";
        var password = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return password.ToString();
            }
            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Length--;
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        }
    }
}
