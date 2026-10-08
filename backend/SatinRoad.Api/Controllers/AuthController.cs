using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatinRoad.Api.Auth;
using SatinRoad.Api.Contracts;
using SatinRoad.Core.Auth;
using UserEntity = SatinRoad.Core.Entities.User;

namespace SatinRoad.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService auth, JwtTokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var user = await auth.RegisterAsync(request.Username, request.Password);
        return Created("/api/auth/me", ToResponse(user));
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<AuthResponse> Login(LoginRequest request) =>
        ToResponse(await auth.LoginAsync(request.Username, request.Password));

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<MeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public MeResponse Me() => new(
        User.GetUserId(),
        User.Identity!.Name!,
        User.FindFirstValue(ClaimTypes.Role)!);

    private AuthResponse ToResponse(UserEntity user) =>
        new(tokens.CreateToken(user), user.Id, user.Username, user.Role);
}