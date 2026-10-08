namespace SatinRoad.Api.Contracts;

public record RegisterRequest(string Username, string Password);

public record LoginRequest(string Username, string Password);

public record AuthResponse(string Token, int UserId, string Username, string Role);

public record MeResponse(int Id, string Username, string Role);