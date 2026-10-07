namespace SatinRoad.Core.Vendors;

public interface IVendorRepository
{
    /// <summary>All vendors that have sold at least one order.</summary>
    Task<List<VendorStats>> GetVendorStatsAsync();
}
