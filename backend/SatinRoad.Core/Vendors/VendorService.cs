namespace SatinRoad.Core.Vendors;

public class VendorService(IVendorRepository repo)
{
    // Hard story #13: "If a vendor has sold more than 100 orders they will be featured
    // on the top of the listings / landing page."
    public const int FeaturedAfterOrders = 100;

    /// <summary>True when the vendor sold MORE than FeaturedAfterOrders orders.</summary>
    public static bool IsFeatured(int ordersSold) => ordersSold > FeaturedAfterOrders;

    /// <summary>Featured vendors that were not shut down by the FBI, best sellers first.</summary>
    public async Task<List<FeaturedVendor>> GetFeaturedAsync()
    {
        var stats = await repo.GetVendorStatsAsync();

        return stats
            .Where(v => !v.IsShutDown && IsFeatured(v.OrdersSold))
            .OrderByDescending(v => v.OrdersSold)
            .ThenBy(v => v.Username)
            .Select(v => new FeaturedVendor(v.VendorId, v.Username, v.OrdersSold))
            .ToList();
    }
}
