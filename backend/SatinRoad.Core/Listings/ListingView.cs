namespace SatinRoad.Core.Listings;

public record ListingView(
    int Id,
    int VendorId,
    string VendorName,
    int CategoryId,
    string CategoryName,
    string Title,
    string? Description,
    decimal Price,
    int Stock,
    bool IsActive,
    DateTime CreatedAt);