using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Auth;

public class AuthService(IUserRepository users, IPasswordHasher hasher)
{
    public const int MinPasswordLength = 6;

    public async Task<User> RegisterAsync(string? username, string? password)
    {
        var cleanUsername = username?.Trim() ?? "";

        if (cleanUsername.Length == 0)
            throw new ValidationException("Username is required.");

        if (password is null || password.Length < MinPasswordLength)
            throw new ValidationException(
                $"Password must be at least {MinPasswordLength} characters long.");

        if (await users.GetByUsernameAsync(cleanUsername) is not null)
            throw new ConflictException($"Username '{cleanUsername}' is already taken.");

        var user = new User
        {
            Username = cleanUsername,
            PasswordHash = hasher.Hash(password),
        };

        return await users.AddAsync(user);
    }

    public async Task<User> LoginAsync(string? username, string? password)
    {
        var user = await users.GetByUsernameAsync(username?.Trim() ?? "");

        if (user is null || password is null || !hasher.Verify(password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");

        if (user.IsShutDown)
            throw new ForbiddenException("This account was shut down by the FBI.");

        return user;
    }
}