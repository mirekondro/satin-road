using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;
using SatinRoad.Core.Orders;

namespace SatinRoad.Tests.Orders;

// Issue #10 – placing an order
public class OrderServiceTests
{
    private readonly FakeOrderRepository _repo = new();
    private readonly OrderService _service;
    private readonly User _buyer;
    private readonly User _vendor;

    public OrderServiceTests()
    {
        _service = new OrderService(_repo, new FakeChanceProvider(false), new FbiSettings());
        _buyer = _repo.SeedUser("buyer");
        _vendor = _repo.SeedUser("vendor");
    }

    [Fact]
    public async Task PlaceOrder_Valid_SavesOrderAndDecreasesStock()
    {
        var listing = _repo.SeedListing(_vendor.Id, stock: 5);

        var result = await _service.PlaceOrderAsync(_buyer.Id, listing.Id, 2);

        Assert.Single(_repo.Orders);
        Assert.Equal(3, listing.Stock);
        Assert.Equal(_buyer.Id, result.Order.BuyerId);
        Assert.Equal(_vendor.Id, result.Order.VendorId);
        Assert.Equal(listing.Id, result.Order.ListingId);
        Assert.False(result.VendorShutDown);
    }

    [Fact]
    public async Task PlaceOrder_StoresUnitPriceAndTotal()
    {
        var listing = _repo.SeedListing(_vendor.Id, price: 12.50m);

        var result = await _service.PlaceOrderAsync(_buyer.Id, listing.Id, 3);

        Assert.Equal(12.50m, result.Order.UnitPrice);
        Assert.Equal(37.50m, result.Order.Total);
        Assert.False(result.Order.DiscountApplied);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(OrderService.MaxQuantity + 1)]
    public async Task PlaceOrder_InvalidQuantity_ThrowsValidation(int quantity)
    {
        var listing = _repo.SeedListing(_vendor.Id, stock: 500);

        await Assert.ThrowsAsync<ValidationException>(
            () => _service.PlaceOrderAsync(_buyer.Id, listing.Id, quantity));
        Assert.Empty(_repo.Orders);
    }

    [Fact]
    public async Task PlaceOrder_UnknownListing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.PlaceOrderAsync(_buyer.Id, 999, 1));
    }

    [Fact]
    public async Task PlaceOrder_InactiveListing_ThrowsNotFound()
    {
        var listing = _repo.SeedListing(_vendor.Id, isActive: false);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.PlaceOrderAsync(_buyer.Id, listing.Id, 1));
    }

    [Fact]
    public async Task PlaceOrder_OwnListing_ThrowsValidation()
    {
        var listing = _repo.SeedListing(_vendor.Id);

        await Assert.ThrowsAsync<ValidationException>(
            () => _service.PlaceOrderAsync(_vendor.Id, listing.Id, 1));
    }

    [Fact]
    public async Task PlaceOrder_ShutDownVendor_ThrowsNotFound()
    {
        var busted = _repo.SeedUser("busted", isShutDown: true);
        var listing = _repo.SeedListing(busted.Id);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.PlaceOrderAsync(_buyer.Id, listing.Id, 1));
    }

    [Fact]
    public async Task PlaceOrder_NotEnoughStock_ThrowsConflict()
    {
        var listing = _repo.SeedListing(_vendor.Id, stock: 2);

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.PlaceOrderAsync(_buyer.Id, listing.Id, 3));
        Assert.Equal(2, listing.Stock);
        Assert.Empty(_repo.Orders);
    }

    [Fact]
    public async Task PlaceOrder_ExactlyRemainingStock_Succeeds()
    {
        var listing = _repo.SeedListing(_vendor.Id, stock: 3);

        await _service.PlaceOrderAsync(_buyer.Id, listing.Id, 3);

        Assert.Equal(0, listing.Stock);
    }
}
