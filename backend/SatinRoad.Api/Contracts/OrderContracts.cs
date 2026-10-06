namespace SatinRoad.Api.Contracts;

public record OrderRequest(int ListingId, int Quantity);

public record PlacedOrderResponse(
    int Id,
    int ListingId,
    int Quantity,
    decimal UnitPrice,
    bool DiscountApplied,
    decimal Total,
    bool VendorShutDown);
