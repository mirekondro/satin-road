namespace SatinRoad.Core.Listings;

public class ListingService(IListingRepository repo)
{
    public const int TitleMinLength = 3;
    public const int TitleMaxLength = 100;
    public const int DescriptionMaxLength = 1000;
    public const decimal MaxPrice = 1_000_000m;

    public Task<List<ListingView>> GetActiveAsync(int? categoryId = null) => throw new NotImplementedException();
    public Task<List<ListingView>> GetMineAsync(int vendorId) => throw new NotImplementedException();
    public Task<ListingView> GetByIdAsync(int id) => throw new NotImplementedException();

    public Task<ListingView> CreateAsync(int vendorId, ListingInput input) => throw new NotImplementedException();
    public Task<ListingView> UpdateAsync(int vendorId, int listingId, ListingInput input) => throw new NotImplementedException();
    public Task<ListingView> UpdateStockAsync(int vendorId, int listingId, int stock) => throw new NotImplementedException();
    public Task DeactivateAsync(int vendorId, int listingId) => throw new NotImplementedException();
}