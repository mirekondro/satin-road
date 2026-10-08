namespace SatinRoad.Api.Contracts;

public record CategoryDto(int Id, string Name);

public record CategoryRequest(string Name);