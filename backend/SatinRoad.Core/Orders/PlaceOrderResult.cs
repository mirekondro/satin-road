using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Orders;

/// <param name="Order">The saved order.</param>
/// <param name="VendorShutDown">True when the buyer turned out to be FBI (hard story #12).</param>
public record PlaceOrderResult(Order Order, bool VendorShutDown);
