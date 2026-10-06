namespace SatinRoad.Core.Orders;

public record OrderView(
    int Id,
    int ListingId,
    string ListingTitle,
    int BuyerId,
    string BuyerName,
    int VendorId,
    string VendorName,
    int Quantity,
    decimal UnitPrice,
    bool DiscountApplied,
    decimal Total,
    DateTime CreatedAt);
