namespace SatinRoad.Core.Vendors;

public class VendorService(IVendorRepository repo)
{
    // Hard story #13: "If a vendor has sold more than 100 orders they will be featured
    // on the top of the listings / landing page."
    public const int FeaturedAfterOrders = 100;

    /// <summary>TODO #13: true when the vendor sold MORE than FeaturedAfterOrders orders.</summary>
    public static bool IsFeatured(int ordersSold) => throw new NotImplementedException();

    /// <summary>
    /// TODO #13: vendors that are featured and not shut down by the FBI,
    /// best sellers first (then by username).
    /// </summary>
    public Task<List<FeaturedVendor>> GetFeaturedAsync() => throw new NotImplementedException();
}
