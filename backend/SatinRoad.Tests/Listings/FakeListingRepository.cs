using SatinRoad.Core.Entities;
using SatinRoad.Core.Listings;

namespace SatinRoad.Tests.Listings;

public class FakeListingRepository : IListingRepository
{
    public List<Listing> Listings { get; } = [];

    public Dictionary<int, string> Categories { get; } = new()
    {
        [1] = "Drugs",
        [2] = "Weapons",
    };

    private int _nextId = 1;

    public Task<List<ListingView>> GetActiveAsync(int? categoryId) =>
        Task.FromResult(Listings
            .Where(l => l.IsActive && (categoryId == null || l.CategoryId == categoryId))
            .Select(ToView)
            .ToList());

    public Task<List<ListingView>> GetByVendorAsync(int vendorId) =>
        Task.FromResult(Listings.Where(l => l.VendorId == vendorId).Select(ToView).ToList());

    public Task<ListingView?> GetViewByIdAsync(int id) =>
        Task.FromResult(Listings.Where(l => l.Id == id).Select(ToView).FirstOrDefault());

    public Task<Listing?> GetByIdAsync(int id) =>
        Task.FromResult(Listings.FirstOrDefault(l => l.Id == id));

    public Task<bool> CategoryExistsAsync(int categoryId) =>
        Task.FromResult(Categories.ContainsKey(categoryId));

    public Task<Listing> AddAsync(Listing listing)
    {
        listing.Id = _nextId++;
        Listings.Add(listing);
        return Task.FromResult(listing);
    }

    public Task UpdateAsync(Listing listing) => Task.CompletedTask; // objekt v listu už je změněný

    public Listing Seed(int vendorId, string title = "Test item", int categoryId = 1,
        decimal price = 10m, int stock = 5, bool isActive = true)
    {
        var l = new Listing
        {
            Id = _nextId++, VendorId = vendorId, CategoryId = categoryId,
            Title = title, Price = price, Stock = stock, IsActive = isActive,
        };
        Listings.Add(l);
        return l;
    }

    private ListingView ToView(Listing l) => new(
        l.Id, l.VendorId, $"vendor{l.VendorId}",
        l.CategoryId, Categories.GetValueOrDefault(l.CategoryId, "?"),
        l.Title, l.Description, l.Price, l.Stock, l.IsActive, l.CreatedAt);
}