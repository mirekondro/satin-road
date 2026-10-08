using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Listings;

public interface IListingRepository
{
    Task<List<ListingView>> GetActiveAsync(int? categoryId);
    Task<List<ListingView>> GetByVendorAsync(int vendorId);
    Task<ListingView?> GetViewByIdAsync(int id);

    Task<Listing?> GetByIdAsync(int id);
    Task<bool> CategoryExistsAsync(int categoryId);
    Task<Listing> AddAsync(Listing listing);
    Task UpdateAsync(Listing listing);
}