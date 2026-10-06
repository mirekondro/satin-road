using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;
using SatinRoad.Core.Orders;

namespace SatinRoad.Tests.Orders;

// Issue #12 – hard story: "For every purchase there's a 1% chance the buyer is FBI and the vendor
// will be shut down permanently / their products will be removed from Satin Road."
public class FbiRaidTests
{
    private readonly FakeOrderRepository _repo = new();
    private readonly User _buyer;
    private readonly User _vendor;

    public FbiRaidTests()
    {
        _buyer = _repo.SeedUser("buyer");
        _vendor = _repo.SeedUser("vendor");
    }

    private OrderService Service(FakeChanceProvider chance, double fbiChance = 0.01) =>
        new(_repo, chance, new FbiSettings(fbiChance));

    [Fact]
    public async Task PlaceOrder_NoRaid_VendorStaysActive()
    {
        var listing = _repo.SeedListing(_vendor.Id);

        var result = await Service(new FakeChanceProvider(false)).PlaceOrderAsync(_buyer.Id, listing.Id, 1);

        Assert.False(result.VendorShutDown);
        Assert.False(_vendor.IsShutDown);
        Assert.True(listing.IsActive);
    }

    [Fact]
    public async Task PlaceOrder_Raid_ShutsDownVendor()
    {
        var listing = _repo.SeedListing(_vendor.Id);

        var result = await Service(new FakeChanceProvider(true)).PlaceOrderAsync(_buyer.Id, listing.Id, 1);

        Assert.True(result.VendorShutDown);
        Assert.True(_vendor.IsShutDown);
    }

    [Fact]
    public async Task PlaceOrder_Raid_RemovesAllProductsOfVendor()
    {
        var bought = _repo.SeedListing(_vendor.Id);
        var other1 = _repo.SeedListing(_vendor.Id);
        var other2 = _repo.SeedListing(_vendor.Id);

        await Service(new FakeChanceProvider(true)).PlaceOrderAsync(_buyer.Id, bought.Id, 1);

        Assert.All(new[] { bought, other1, other2 }, l => Assert.False(l.IsActive));
    }

    [Fact]
    public async Task PlaceOrder_Raid_OtherVendorsAreNotAffected()
    {
        var otherVendor = _repo.SeedUser("other-vendor");
        var otherListing = _repo.SeedListing(otherVendor.Id);
        var listing = _repo.SeedListing(_vendor.Id);

        await Service(new FakeChanceProvider(true)).PlaceOrderAsync(_buyer.Id, listing.Id, 1);

        Assert.False(otherVendor.IsShutDown);
        Assert.True(otherListing.IsActive);
    }

    [Fact]
    public async Task PlaceOrder_Raid_OrderIsStillSaved()
    {
        var listing = _repo.SeedListing(_vendor.Id, stock: 5);

        await Service(new FakeChanceProvider(true)).PlaceOrderAsync(_buyer.Id, listing.Id, 2);

        Assert.Single(_repo.Orders);
        Assert.Equal(3, listing.Stock);
    }

    [Fact]
    public async Task PlaceOrder_AfterRaid_VendorListingsCannotBeBought()
    {
        var first = _repo.SeedListing(_vendor.Id);
        var second = _repo.SeedListing(_vendor.Id);
        await Service(new FakeChanceProvider(true)).PlaceOrderAsync(_buyer.Id, first.Id, 1);

        await Assert.ThrowsAsync<NotFoundException>(
            () => Service(new FakeChanceProvider(false)).PlaceOrderAsync(_buyer.Id, second.Id, 1));
    }

    [Fact]
    public async Task PlaceOrder_RollsOnceWithConfiguredProbability()
    {
        var listing = _repo.SeedListing(_vendor.Id);
        var chance = new FakeChanceProvider(false);

        await Service(chance, fbiChance: 0.01).PlaceOrderAsync(_buyer.Id, listing.Id, 1);

        Assert.Equal(1, chance.Rolls);
        Assert.Equal(0.01, chance.LastProbability);
    }

    [Fact]
    public async Task PlaceOrder_InvalidOrder_DoesNotRollForFbi()
    {
        var listing = _repo.SeedListing(_vendor.Id, stock: 1);
        var chance = new FakeChanceProvider(true);

        await Assert.ThrowsAsync<ConflictException>(
            () => Service(chance).PlaceOrderAsync(_buyer.Id, listing.Id, 5));

        Assert.Equal(0, chance.Rolls);
        Assert.False(_vendor.IsShutDown);
    }
}
