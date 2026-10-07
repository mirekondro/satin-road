using LinqToDB;
using LinqToDB.Async;
using SatinRoad.Core.Vendors;

namespace SatinRoad.Core.Data;

public class VendorRepository(AppDataConnection db) : IVendorRepository
{
    public Task<List<VendorStats>> GetVendorStatsAsync() =>
        (from u in db.Users
         let sold = db.Orders.Count(o => o.VendorId == u.Id)
         where sold > 0
         select new VendorStats(u.Id, u.Username, sold, u.IsShutDown))
        .ToListAsync();
}
