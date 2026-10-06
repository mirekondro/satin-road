using SatinRoad.Core.Entities;
using SatinRoad.Core.Orders;

namespace SatinRoad.Tests.Orders;

// Issue #11 – hard story: "If more than 10 orders is placed on a single vendor by a user,
// the next order price will be reduced by 20%."
public class DiscountTests
{
    private readonly FakeOrderRepository _repo = new();
    private readonly OrderService _service;
    private readonly User _buyer;
    private readonly User _vendor;

    public DiscountTests()
    {
        _service = new OrderService(_repo);
        _buyer = _repo.SeedUser("buyer");
        _vendor = _repo.SeedUser("vendor");
    }

    // ---------- The rule itself (pure function) ----------

    [Theory]
    [InlineData(0, false)]
    [InlineData(9, false)]
    [InlineData(10, false)] // exactly 10 is not "more than 10"
    [InlineData(11, true)]
    [InlineData(50, true)]
    public void QualifiesForDiscount_BoundaryValues(int previousOrders, bool expected)
    {
        Assert.Equal(expected, OrderService.QualifiesForDiscount(previousOrders));
    }

    [Theory]
    [InlineData(10.00, 1, false, 10.00)]
    [InlineData(10.00, 1, true, 8.00)]
    [InlineData(10.00, 3, true, 24.00)]
    [InlineData(9.99, 1, true, 7.99)] // 7.992 rounded to 2 decimals
    public void CalculateTotal_AppliesDiscountAndRounds(
        decimal unitPrice, int quantity, bool discount, decimal expected)
    {
        Assert.Equal(expected, OrderService.CalculateTotal(unitPrice, quantity, discount));
    }

    // ---------- The rule inside PlaceOrder ----------

    [Fact]
    public async Task PlaceOrder_With10PreviousOrders_NoDiscount()
    {
        var listing = _repo.SeedListing(_vendor.Id, price: 10m);
        _repo.SeedOrders(_buyer.Id, _vendor.Id, 10);

        var result = await _service.PlaceOrderAsync(_buyer.Id, listing.Id, 1);

        Assert.False(result.Order.DiscountApplied);
        Assert.Equal(10m, result.Order.Total);
    }

    [Fact]
    public async Task PlaceOrder_With11PreviousOrders_Gets20PercentOff()
    {
        var listing = _repo.SeedListing(_vendor.Id, price: 10m);
        _repo.SeedOrders(_buyer.Id, _vendor.Id, 11);

        var result = await _service.PlaceOrderAsync(_buyer.Id, listing.Id, 1);

        Assert.True(result.Order.DiscountApplied);
        Assert.Equal(10m, result.Order.UnitPrice); // unit price stays the original
        Assert.Equal(8m, result.Order.Total);
    }

    [Fact]
    public async Task PlaceOrder_OrdersAtOtherVendorDoNotCount()
    {
        var otherVendor = _repo.SeedUser("other-vendor");
        var listing = _repo.SeedListing(_vendor.Id, price: 10m);
        _repo.SeedOrders(_buyer.Id, otherVendor.Id, 20);

        var result = await _service.PlaceOrderAsync(_buyer.Id, listing.Id, 1);

        Assert.False(result.Order.DiscountApplied);
    }

    [Fact]
    public async Task PlaceOrder_OrdersOfOtherBuyersDoNotCount()
    {
        var otherBuyer = _repo.SeedUser("other-buyer");
        var listing = _repo.SeedListing(_vendor.Id, price: 10m);
        _repo.SeedOrders(otherBuyer.Id, _vendor.Id, 20);

        var result = await _service.PlaceOrderAsync(_buyer.Id, listing.Id, 1);

        Assert.False(result.Order.DiscountApplied);
    }
}
