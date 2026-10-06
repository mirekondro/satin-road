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

    public async Task<PlaceOrderResult> PlaceOrderAsync(int buyerId, int listingId, int quantity)
    {
        if (quantity < 1 || quantity > MaxQuantity)
            throw new ValidationException($"Quantity must be between 1 and {MaxQuantity}.");

        var listing = await repo.GetListingAsync(listingId);
        if (listing is null || !listing.IsActive)
            throw new NotFoundException($"Listing {listingId} not found.");

        if (listing.VendorId == buyerId)
            throw new ValidationException("You cannot buy your own listing.");

        // A vendor shut down by the FBI is gone – their listings behave as if they don't exist
        var vendor = await repo.GetUserAsync(listing.VendorId);
        if (vendor is null || vendor.IsShutDown)
            throw new NotFoundException($"Listing {listingId} not found.");

        if (listing.Stock < quantity)
            throw new ConflictException($"Not enough stock. Only {listing.Stock} left.");

        // Hard story #11 – loyalty discount at this vendor
        var previousOrders = await repo.CountOrdersAsync(buyerId, listing.VendorId);
        var discount = QualifiesForDiscount(previousOrders);

        var order = await repo.PlaceAsync(new Order
        {
            BuyerId = buyerId,
            VendorId = listing.VendorId,
            ListingId = listing.Id,
            Quantity = quantity,
            UnitPrice = listing.Price,
            DiscountApplied = discount,
            Total = CalculateTotal(listing.Price, quantity, discount),
        });

        return new PlaceOrderResult(order, VendorShutDown: false);
    }

    // ---------- Hard story #11: 20% discount ----------

    /// <summary>True when the buyer already has MORE than DiscountAfterOrders orders at this vendor.</summary>
    public static bool QualifiesForDiscount(int previousOrdersAtVendor) =>
        previousOrdersAtVendor > DiscountAfterOrders;

    /// <summary>unitPrice * quantity, minus DiscountRate when discounted, rounded to 2 decimals.</summary>
    public static decimal CalculateTotal(decimal unitPrice, int quantity, bool discount)
    {
        var total = unitPrice * quantity;

        if (discount)
            total *= 1 - DiscountRate;

        return decimal.Round(total, 2);
    }
}
