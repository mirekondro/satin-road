using LinqToDB;
using LinqToDB.Async;
using SatinRoad.Core.Entities;
using SatinRoad.Core.Listings;

namespace SatinRoad.Core.Data;

public class ListingRepository(AppDataConnection db) : IListingRepository
{
    public async Task<List<ListingView>> GetActiveAsync(int? categoryId)
    {
        var listings = db.Listings.Where(l =>
            l.IsActive &&
            !db.Users.Any(u => u.Id == l.VendorId && u.IsShutDown));

        if (categoryId.HasValue)
            listings = listings.Where(l => l.CategoryId == categoryId.Value);

        var result = await ToViews(listings).ToListAsync();
        return result.OrderByDescending(v => v.CreatedAt).ToList(); 
    }

    public async Task<List<ListingView>> GetByVendorAsync(int vendorId)
    {
        var result = await ToViews(db.Listings.Where(l => l.VendorId == vendorId)).ToListAsync();
        return result.OrderByDescending(v => v.CreatedAt).ToList();
    }

    public Task<ListingView?> GetViewByIdAsync(int id) =>
        ToViews(db.Listings.Where(l => l.Id == id)).FirstOrDefaultAsync();

    public Task<Listing?> GetByIdAsync(int id) =>
        db.Listings.FirstOrDefaultAsync(l => l.Id == id);

    public Task<bool> CategoryExistsAsync(int categoryId) =>
        db.Categories.AnyAsync(c => c.Id == categoryId);

    public async Task<Listing> AddAsync(Listing listing)
    {
        listing.Id = await db.InsertWithInt32IdentityAsync(listing);
        return listing;
    }

    public Task UpdateAsync(Listing listing) =>
        db.UpdateAsync(listing);

    private IQueryable<ListingView> ToViews(IQueryable<Listing> listings) =>
        from l in listings
        join u in db.Users on l.VendorId equals u.Id
        join c in db.Categories on l.CategoryId equals c.Id
        select new ListingView(
            l.Id, l.VendorId, u.Username,
            l.CategoryId, c.Name,
            l.Title, l.Description, l.Price, l.Stock,
            l.IsActive, l.CreatedAt);
}