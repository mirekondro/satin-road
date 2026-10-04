using LinqToDB;
using LinqToDB.Async;
using SatinRoad.Core.Auth;
using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Data;

public class UserRepository(AppDataConnection db) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username) =>
        db.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User> AddAsync(User user)
    {
        user.Id = await db.InsertWithInt32IdentityAsync(user);
        return user;
    }
}
