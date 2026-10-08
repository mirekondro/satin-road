using SatinRoad.Core.Auth;
using SatinRoad.Core.Common;

namespace SatinRoad.Tests.Auth;

public class AuthServiceTests
{
    private readonly FakeUserRepository _repo = new();
    private readonly BcryptPasswordHasher _hasher = new();
    private readonly AuthService _service;

    public AuthServiceTests() => _service = new AuthService(_repo, _hasher);

    // ---------- Register ----------

    [Fact]
    public async Task Register_ValidInput_SavesUserWithRoleUser()
    {
        var user = await _service.RegisterAsync("bob", "secret123");

        Assert.Single(_repo.Items);
        Assert.Equal("bob", user.Username);
        Assert.Equal("User", user.Role);
        Assert.False(user.IsShutDown);
    }

    [Fact]
    public async Task Register_TrimsUsername()
    {
        var user = await _service.RegisterAsync("  bob  ", "secret123");

        Assert.Equal("bob", user.Username);
    }

    [Fact]
    public async Task Register_StoresHashNotPlainPassword()
    {
        var user = await _service.RegisterAsync("bob", "secret123");

        Assert.NotEqual("secret123", user.PasswordHash);
        Assert.True(_hasher.Verify("secret123", user.PasswordHash));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Register_MissingUsername_ThrowsValidation(string? username)
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.RegisterAsync(username, "secret123"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")] // one character below the minimum
    public async Task Register_TooShortPassword_ThrowsValidation(string? password)
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.RegisterAsync("bob", password));
    }

    [Fact]
    public async Task Register_PasswordWithMinimumLength_Succeeds()
    {
        var user = await _service.RegisterAsync("bob", "123456");

        Assert.Equal("bob", user.Username);
    }

    [Fact]
    public async Task Register_DuplicateUsername_ThrowsConflict()
    {
        _repo.Seed("bob", _hasher.Hash("secret123"));

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.RegisterAsync("bob", "other123"));
        Assert.Single(_repo.Items);
    }

    // ---------- Login ----------

    [Fact]
    public async Task Login_CorrectCredentials_ReturnsUser()
    {
        var seeded = _repo.Seed("bob", _hasher.Hash("secret123"));

        var user = await _service.LoginAsync("bob", "secret123");

        Assert.Equal(seeded.Id, user.Id);
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorized()
    {
        _repo.Seed("bob", _hasher.Hash("secret123"));

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _service.LoginAsync("bob", "wrong-password"));
    }

    [Fact]
    public async Task Login_UnknownUser_ThrowsUnauthorized()
    {
        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _service.LoginAsync("nobody", "secret123"));
    }

    [Fact]
    public async Task Login_ShutDownUser_ThrowsForbidden()
    {
        var user = _repo.Seed("bob", _hasher.Hash("secret123"));
        user.IsShutDown = true;

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.LoginAsync("bob", "secret123"));
    }
}
