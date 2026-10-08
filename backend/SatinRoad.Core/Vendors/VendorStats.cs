namespace SatinRoad.Core.Vendors;

/// <summary>Raw numbers from the database: how many orders a vendor has sold.</summary>
public record VendorStats(int VendorId, string Username, int OrdersSold, bool IsShutDown);
