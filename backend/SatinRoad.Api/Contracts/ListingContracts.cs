namespace SatinRoad.Api.Contracts;

public record ListingRequest(int CategoryId, string Title, string? Description, decimal Price, int Stock);

public record StockRequest(int Stock);