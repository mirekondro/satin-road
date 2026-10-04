using SatinRoad.Core.Auth;
using SatinRoad.Core.Entities;

namespace SatinRoad.Tests.Auth;

public class FakeUserRepository : IUserRepository
{
    public List<User> Items { get; } = [];

    private int _nextId = 1;

    public Task<User?> GetByUsernameAsync(string username) =>
        Task.FromResult(Items.FirstOrDefault(u => u.Username == username));

    public Task<User> AddAsync(User user)
    {
        user.Id = _nextId++;
        Items.Add(user);
        return Task.FromResult(user);
    }

    public User Seed(string username, string passwordHash, string role = "User")
    {
        var user = new User { Id = _nextId++, Username = username, PasswordHash = passwordHash, Role = role };
        Items.Add(user);
        return user;
    }
}
