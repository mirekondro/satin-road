using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Auth;

public class AuthService(IUserRepository users, IPasswordHasher hasher)
{
    public const int MinPasswordLength = 6;

    // TODO (TDD green): make AuthServiceTests pass
    // 1. username = username?.Trim(); empty → ValidationException
    // 2. password null or shorter than MinPasswordLength → ValidationException
    // 3. await users.GetByUsernameAsync(username) is not null → ConflictException
    // 4. return await users.AddAsync(new User { Username = ..., PasswordHash = hasher.Hash(password) });
    public Task<User> RegisterAsync(string? username, string? password) =>
        throw new NotImplementedException();

    // TODO (TDD green):
    // 1. user = await users.GetByUsernameAsync(username?.Trim() ?? "")
    // 2. user is null OR !hasher.Verify(password, user.PasswordHash) → UnauthorizedException("Invalid username or password.")
    // 3. user.IsShutDown → ForbiddenException("This account was shut down by the FBI.")
    // 4. return user;
    public Task<User> LoginAsync(string? username, string? password) =>
        throw new NotImplementedException();
}
