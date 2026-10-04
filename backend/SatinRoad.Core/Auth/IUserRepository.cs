using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Auth;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddAsync(User user);
}
