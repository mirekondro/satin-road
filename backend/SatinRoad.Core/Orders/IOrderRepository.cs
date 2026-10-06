using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Orders;

public interface IOrderRepository
{
    Task<Listing?> GetListingAsync(int listingId);
    Task<User?> GetUserAsync(int userId);

    /// <summary>How many orders the buyer has already placed at this vendor (20% discount rule).</summary>
    Task<int> CountOrdersAsync(int buyerId, int vendorId);

    /// <summary>Saves the order and decreases the listing stock in ONE transaction.
    /// Throws ConflictException when the stock is no longer sufficient.</summary>
    Task<Order> PlaceAsync(Order order);

    Task<List<OrderView>> GetByBuyerAsync(int buyerId);
    Task<List<OrderView>> GetByVendorAsync(int vendorId);
}
