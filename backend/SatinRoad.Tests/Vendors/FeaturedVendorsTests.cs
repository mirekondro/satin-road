using SatinRoad.Core.Vendors;

namespace SatinRoad.Tests.Vendors;

// Issue #13 – hard story: "If a vendor has sold more than 100 orders they will be featured
// on the top of the listings / landing page."
public class FeaturedVendorsTests
{
    private readonly FakeVendorRepository _repo = new();
    private readonly VendorService _service;

    public FeaturedVendorsTests() => _service = new VendorService(_repo);

    // ---------- The rule itself ----------

    [Theory]
    [InlineData(0, false)]
    [InlineData(99, false)]
    [InlineData(100, false)] // exactly 100 is not "more than 100"
    [InlineData(101, true)]
    [InlineData(5000, true)]
    public void IsFeatured_BoundaryValues(int ordersSold, bool expected)
    {
        Assert.Equal(expected, VendorService.IsFeatured(ordersSold));
    }

    // ---------- The featured list ----------

    [Fact]
    public async Task GetFeatured_NoVendors_ReturnsEmpty()
    {
        Assert.Empty(await _service.GetFeaturedAsync());
    }

    [Fact]
    public async Task GetFeatured_ReturnsOnlyVendorsWithMoreThan100Sales()
    {
        _repo.Seed("small", 10);
        _repo.Seed("almost", 100);
        var big = _repo.Seed("big", 101);

        var featured = await _service.GetFeaturedAsync();

        var only = Assert.Single(featured);
        Assert.Equal(big.VendorId, only.VendorId);
        Assert.Equal(101, only.OrdersSold);
    }

    [Fact]
    public async Task GetFeatured_ExcludesVendorsShutDownByFbi()
    {
        _repo.Seed("busted", 500, isShutDown: true);
        _repo.Seed("clean", 150);

        var featured = await _service.GetFeaturedAsync();

        Assert.Equal(new[] { "clean" }, featured.Select(v => v.Username));
    }

    [Fact]
    public async Task GetFeatured_BestSellersFirst()
    {
        _repo.Seed("bronze", 120);
        _repo.Seed("gold", 900);
        _repo.Seed("silver", 300);

        var featured = await _service.GetFeaturedAsync();

        Assert.Equal(new[] { "gold", "silver", "bronze" }, featured.Select(v => v.Username));
    }

    [Fact]
    public async Task GetFeatured_SameSales_SortedByUsername()
    {
        _repo.Seed("zeta", 200);
        _repo.Seed("alpha", 200);

        var featured = await _service.GetFeaturedAsync();

        Assert.Equal(new[] { "alpha", "zeta" }, featured.Select(v => v.Username));
    }
}
