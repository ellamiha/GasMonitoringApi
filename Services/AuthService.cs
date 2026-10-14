using GasMonitoringApi.Auth;
using GasMonitoringApi.Data;
using GasMonitoringApi.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GasMonitoringApi.Services;

public class AuthService
{
    private readonly AppDbContext db;
    private readonly IPasswordHasher<AppUser> passwordHasher;
    private readonly TokenService tokenService;

    public AuthService(AppDbContext db, IPasswordHasher<AppUser> passwordHasher, TokenService tokenService)
    {
        this.db = db;
        this.passwordHasher = passwordHasher;
        this.tokenService = tokenService;
    }

    public Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        return CreateUserAsync(request, UserRoles.User, cancellationToken);
    }

    public Task<UserResponse> CreateAdminAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        return CreateUserAsync(request, UserRoles.Admin, cancellationToken);
    }

    public async Task<UserResponse> ResetAdminPasswordAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedLogin = request.Login.ToUpperInvariant();
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.NormalizedLogin == normalizedLogin && u.Role == UserRoles.Admin, cancellationToken);
        if (user == null)
            throw new ServiceException(ServiceError.NotFound, "Администратор с таким логином не найден.");
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        await db.SaveChangesAsync(cancellationToken);
        return new UserResponse(user.Id, user.Login, user.Role);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedLogin = request.Login.Trim().ToUpperInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.NormalizedLogin == normalizedLogin, cancellationToken);
        if (user == null)
            throw new ServiceException(ServiceError.Unauthorized, "Неверный логин или пароль.");
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new ServiceException(ServiceError.Unauthorized, "Неверный логин или пароль.");
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            await db.SaveChangesAsync(cancellationToken);
        }
        return tokenService.CreateToken(user);
    }

    public async Task<UserResponse> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        return await db.Users.AsNoTracking().Where(u => u.Id == id)
            .Select(u => new UserResponse(u.Id, u.Login, u.Role))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ServiceException(ServiceError.Unauthorized, "Пользователь не найден.");
    }

    private async Task<UserResponse> CreateUserAsync(
        RegisterRequest request, string role, CancellationToken cancellationToken)
    {
        var normalizedLogin = request.Login.ToUpperInvariant();
        if (await db.Users.AnyAsync(u => u.NormalizedLogin == normalizedLogin, cancellationToken))
            throw new ServiceException(ServiceError.Conflict, "Этот логин уже занят.");
        var user = new AppUser
        {
            Login = request.Login,
            NormalizedLogin = normalizedLogin,
            Role = role
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException error) when (error.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 })
        {
            throw new ServiceException(ServiceError.Conflict, "Этот логин уже занят.");
        }
        return new UserResponse(user.Id, user.Login, user.Role);
    }
}
