using SatinRoad.Core.Vendors;

namespace SatinRoad.Tests.Vendors;

public class FakeVendorRepository : IVendorRepository
{
    public List<VendorStats> Stats { get; } = [];

    private int _nextId = 1;

    public Task<List<VendorStats>> GetVendorStatsAsync() => Task.FromResult(Stats.ToList());

    public VendorStats Seed(string username, int ordersSold, bool isShutDown = false)
    {
        var stats = new VendorStats(_nextId++, username, ordersSold, isShutDown);
        Stats.Add(stats);
        return stats;
    }
}
