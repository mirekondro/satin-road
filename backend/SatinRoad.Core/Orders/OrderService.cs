using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Orders;

public class OrderService(IOrderRepository repo)
{
    public const int MaxQuantity = 100;

    // Hard story #11: "If more than 10 orders is placed on a single vendor by a user,
    // the next order price will be reduced by 20%."
    public const int DiscountAfterOrders = 10;
    public const decimal DiscountRate = 0.20m;

    // ---------- Reading ----------

    public Task<List<OrderView>> GetMineAsync(int buyerId) => repo.GetByBuyerAsync(buyerId);

    public Task<List<OrderView>> GetSalesAsync(int vendorId) => repo.GetByVendorAsync(vendorId);

    // ---------- Placing an order ----------

    public Task<PlaceOrderResult> PlaceOrderAsync(int buyerId, int listingId, int quantity)
    {
        // TODO #10 (make OrderServiceTests green):
        // 1. quantity < 1 or > MaxQuantity                      → ValidationException
        // 2. listing = await repo.GetListingAsync(listingId)
        //    null or !IsActive                                  → NotFoundException
        // 3. listing.VendorId == buyerId                        → ValidationException ("You cannot buy your own listing.")
        // 4. vendor = await repo.GetUserAsync(listing.VendorId)
        //    null or IsShutDown                                 → NotFoundException
        // 5. listing.Stock < quantity                           → ConflictException ("Not enough stock.")
        //
        // TODO #11 (make DiscountTests green):
        // 6. previous = await repo.CountOrdersAsync(buyerId, listing.VendorId)
        //    discount = QualifiesForDiscount(previous)
        // 7. total = CalculateTotal(listing.Price, quantity, discount)
        //
        // 8. var order = new Order { BuyerId, VendorId, ListingId, Quantity, UnitPrice = listing.Price,
        //                            DiscountApplied = discount, Total = total };
        //    order = await repo.PlaceAsync(order);
        //    return new PlaceOrderResult(order, VendorShutDown: false);
        throw new NotImplementedException();
    }

    // ---------- Hard story #11: 20% discount ----------

    /// <summary>True when the buyer already has MORE than DiscountAfterOrders orders at this vendor.</summary>
    public static bool QualifiesForDiscount(int previousOrdersAtVendor) =>
        throw new NotImplementedException(); // TODO #11: one line

    /// <summary>unitPrice * quantity, minus DiscountRate when discounted, rounded to 2 decimals.</summary>
    public static decimal CalculateTotal(decimal unitPrice, int quantity, bool discount) =>
        throw new NotImplementedException(); // TODO #11: use decimal.Round(..., 2)
}
