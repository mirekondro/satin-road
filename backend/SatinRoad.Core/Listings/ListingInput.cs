namespace SatinRoad.Core.Listings;

public record ListingInput(
    int CategoryId,
    string? Title,
    string? Description,
    decimal Price,
    int Stock);