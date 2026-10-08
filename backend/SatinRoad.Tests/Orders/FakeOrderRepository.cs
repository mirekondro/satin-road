using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;
using SatinRoad.Core.Orders;

namespace SatinRoad.Tests.Orders;

public class FakeOrderRepository : IOrderRepository
{
    public List<User> Users { get; } = [];
    public List<Listing> Listings { get; } = [];
    public List<Order> Orders { get; } = [];

    private int _nextId = 1;

    public Task<Listing?> GetListingAsync(int listingId) =>
        Task.FromResult(Listings.FirstOrDefault(l => l.Id == listingId));

    public Task<User?> GetUserAsync(int userId) =>
        Task.FromResult(Users.FirstOrDefault(u => u.Id == userId));

    public Task<int> CountOrdersAsync(int buyerId, int vendorId) =>
        Task.FromResult(Orders.Count(o => o.BuyerId == buyerId && o.VendorId == vendorId));

    public Task<Order> PlaceAsync(Order order)
    {
        var listing = Listings.First(l => l.Id == order.ListingId);
        if (listing.Stock < order.Quantity)
            throw new ConflictException("Not enough stock.");

        listing.Stock -= order.Quantity;
        order.Id = _nextId++;
        Orders.Add(order);
        return Task.FromResult(order);
    }

    public Task ShutDownVendorAsync(int vendorId)
    {
        Users.First(u => u.Id == vendorId).IsShutDown = true;
        foreach (var listing in Listings.Where(l => l.VendorId == vendorId))
            listing.IsActive = false;
        return Task.CompletedTask;
    }

    public Task<List<OrderView>> GetByBuyerAsync(int buyerId) =>
        Task.FromResult(Orders.Where(o => o.BuyerId == buyerId).Select(ToView).ToList());

    public Task<List<OrderView>> GetByVendorAsync(int vendorId) =>
        Task.FromResult(Orders.Where(o => o.VendorId == vendorId).Select(ToView).ToList());

    // ---------- Seed helpers ----------

    public User SeedUser(string username, bool isShutDown = false)
    {
        var user = new User { Id = _nextId++, Username = username, PasswordHash = "x", IsShutDown = isShutDown };
        Users.Add(user);
        return user;
    }

    public Listing SeedListing(int vendorId, decimal price = 10m, int stock = 5, bool isActive = true)
    {
        var listing = new Listing
        {
            Id = _nextId++, VendorId = vendorId, CategoryId = 1, Title = "Test item",
            Price = price, Stock = stock, IsActive = isActive,
        };
        Listings.Add(listing);
        return listing;
    }

    /// <summary>Adds <paramref name="count"/> past orders of the buyer at the vendor.</summary>
    public void SeedOrders(int buyerId, int vendorId, int count)
    {
        for (var i = 0; i < count; i++)
            Orders.Add(new Order
            {
                Id = _nextId++, BuyerId = buyerId, VendorId = vendorId, ListingId = 0,
                Quantity = 1, UnitPrice = 1m, Total = 1m,
            });
    }

    private OrderView ToView(Order o) => new(
        o.Id, o.ListingId, "Test item",
        o.BuyerId, $"user{o.BuyerId}",
        o.VendorId, $"user{o.VendorId}",
        o.Quantity, o.UnitPrice, o.DiscountApplied, o.Total, o.CreatedAt);
}
