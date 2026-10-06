using LinqToDB;
using LinqToDB.Async;
using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;
using SatinRoad.Core.Orders;

namespace SatinRoad.Core.Data;

public class OrderRepository(AppDataConnection db) : IOrderRepository
{
    public Task<Listing?> GetListingAsync(int listingId) =>
        db.Listings.FirstOrDefaultAsync(l => l.Id == listingId);

    public Task<User?> GetUserAsync(int userId) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == userId);

    public Task<int> CountOrdersAsync(int buyerId, int vendorId) =>
        db.Orders.CountAsync(o => o.BuyerId == buyerId && o.VendorId == vendorId);

    public async Task<Order> PlaceAsync(Order order)
    {
        await using var tx = await db.BeginTransactionAsync();

        // Decrease stock only if there is still enough – safe even when two people buy at once
        var updated = await db.Listings
            .Where(l => l.Id == order.ListingId && l.Stock >= order.Quantity)
            .Set(l => l.Stock, l => l.Stock - order.Quantity)
            .UpdateAsync();

        if (updated == 0)
            throw new ConflictException("Not enough stock.");

        order.Id = await db.InsertWithInt32IdentityAsync(order);

        await tx.CommitAsync();
        return order;
    }

    public async Task<List<OrderView>> GetByBuyerAsync(int buyerId)
    {
        var result = await ToViews(db.Orders.Where(o => o.BuyerId == buyerId)).ToListAsync();
        return result.OrderByDescending(o => o.CreatedAt).ToList();
    }

    public async Task<List<OrderView>> GetByVendorAsync(int vendorId)
    {
        var result = await ToViews(db.Orders.Where(o => o.VendorId == vendorId)).ToListAsync();
        return result.OrderByDescending(o => o.CreatedAt).ToList();
    }

    private IQueryable<OrderView> ToViews(IQueryable<Order> orders) =>
        from o in orders
        join l in db.Listings on o.ListingId equals l.Id
        join b in db.Users on o.BuyerId equals b.Id
        join v in db.Users on o.VendorId equals v.Id
        select new OrderView(
            o.Id, o.ListingId, l.Title,
            o.BuyerId, b.Username,
            o.VendorId, v.Username,
            o.Quantity, o.UnitPrice, o.DiscountApplied, o.Total,
            o.CreatedAt);
}
