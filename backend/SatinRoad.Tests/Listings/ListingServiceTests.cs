using SatinRoad.Core.Common;
using SatinRoad.Core.Listings;

namespace SatinRoad.Tests.Listings;

public class ListingServiceTests
{
    private const int Vendor = 1;
    private const int OtherVendor = 2;

    private readonly FakeListingRepository _repo = new();
    private readonly ListingService _service;

    public ListingServiceTests() => _service = new ListingService(_repo);

    // Platný vstup – v testech z něj dělám varianty pomocí "with"
    private static ListingInput Valid() =>
        new(CategoryId: 1, Title: "Fake Rolex", Description: "Ticks almost like a real one",
            Price: 99.99m, Stock: 10);

    // ---------- Create ----------

    [Fact]
    public async Task Create_ValidInput_SavesActiveListingForVendor()
    {
        var created = await _service.CreateAsync(Vendor, Valid() with { Title = "  Fake Rolex  " });

        Assert.Equal("Fake Rolex", created.Title);
        Assert.Equal(Vendor, created.VendorId);
        Assert.Equal("Drugs", created.CategoryName);
        Assert.True(created.IsActive);
        Assert.Single(_repo.Listings);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")] // kratší než 3
    public async Task Create_InvalidTitle_ThrowsValidation(string? title)
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateAsync(Vendor, Valid() with { Title = title }));
        Assert.Empty(_repo.Listings);
    }

    [Fact]
    public async Task Create_TitleLongerThan100_ThrowsValidation()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateAsync(Vendor, Valid() with { Title = new string('x', 101) }));
    }

    [Fact]
    public async Task Create_DescriptionLongerThan1000_ThrowsValidation()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateAsync(Vendor, Valid() with { Description = new string('x', 1001) }));
    }

    [Fact]
    public async Task Create_WhitespaceDescription_IsStoredAsNull()
    {
        var created = await _service.CreateAsync(Vendor, Valid() with { Description = "   " });
        Assert.Null(created.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(1_000_000.01)] // nad limitem
    [InlineData(9.999)]        // víc než 2 desetinná místa
    public async Task Create_InvalidPrice_ThrowsValidation(double price)
    {
        // decimal nejde dát přímo do [InlineData], proto double + převod
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateAsync(Vendor, Valid() with { Price = (decimal)price }));
    }

    [Fact]
    public async Task Create_MaxPrice_IsAllowed() // hraniční hodnota
    {
        var created = await _service.CreateAsync(Vendor, Valid() with { Price = 1_000_000m });
        Assert.Equal(1_000_000m, created.Price);
    }

    [Fact]
    public async Task Create_NegativeStock_ThrowsValidation()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateAsync(Vendor, Valid() with { Stock = -1 }));
    }

    [Fact]
    public async Task Create_ZeroStock_IsAllowed() // vyprodáno je OK
    {
        var created = await _service.CreateAsync(Vendor, Valid() with { Stock = 0 });
        Assert.Equal(0, created.Stock);
    }

    [Fact]
    public async Task Create_UnknownCategory_ThrowsValidation()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateAsync(Vendor, Valid() with { CategoryId = 999 }));
    }

    // ---------- Update (vlastnictví!) ----------

    [Fact]
    public async Task Update_OwnListing_UpdatesFields()
    {
        var listing = _repo.Seed(Vendor);

        var updated = await _service.UpdateAsync(Vendor, listing.Id,
            Valid() with { Title = "Better Rolex", Price = 150m, CategoryId = 2 });

        Assert.Equal("Better Rolex", updated.Title);
        Assert.Equal(150m, updated.Price);
        Assert.Equal("Weapons", updated.CategoryName);
    }

    [Fact]
    public async Task Update_SomeoneElsesListing_ThrowsForbidden()
    {
        var listing = _repo.Seed(OtherVendor, title: "Original");

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.UpdateAsync(Vendor, listing.Id, Valid()));
        Assert.Equal("Original", listing.Title); // nic se nezměnilo
    }

    [Fact]
    public async Task Update_NonExistingListing_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAsync(Vendor, 999, Valid()));
    }

    [Fact]
    public async Task Update_DeactivatedListing_ThrowsNotFound()
    {
        var listing = _repo.Seed(Vendor, isActive: false);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAsync(Vendor, listing.Id, Valid()));
    }

    [Fact]
    public async Task Update_InvalidInput_ThrowsValidation()
    {
        var listing = _repo.Seed(Vendor);

        await Assert.ThrowsAsync<ValidationException>(
            () => _service.UpdateAsync(Vendor, listing.Id, Valid() with { Price = 0 }));
    }

    // ---------- Stock (správa inventáře) ----------

    [Fact]
    public async Task UpdateStock_OwnListing_SetsStock()
    {
        var listing = _repo.Seed(Vendor, stock: 5);

        var updated = await _service.UpdateStockAsync(Vendor, listing.Id, 42);

        Assert.Equal(42, updated.Stock);
    }

    [Fact]
    public async Task UpdateStock_Negative_ThrowsValidation()
    {
        var listing = _repo.Seed(Vendor, stock: 5);

        await Assert.ThrowsAsync<ValidationException>(
            () => _service.UpdateStockAsync(Vendor, listing.Id, -1));
        Assert.Equal(5, listing.Stock);
    }

    [Fact]
    public async Task UpdateStock_SomeoneElsesListing_ThrowsForbidden()
    {
        var listing = _repo.Seed(OtherVendor);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.UpdateStockAsync(Vendor, listing.Id, 10));
    }

    // ---------- Deactivate (= "smazání") ----------

    [Fact]
    public async Task Deactivate_OwnListing_HidesItButKeepsItInDb()
    {
        var listing = _repo.Seed(Vendor);

        await _service.DeactivateAsync(Vendor, listing.Id);

        Assert.False(listing.IsActive);
        Assert.Single(_repo.Listings); // nesmazáno – objednávky na něj odkazují
    }

    [Fact]
    public async Task Deactivate_SomeoneElsesListing_ThrowsForbidden()
    {
        var listing = _repo.Seed(OtherVendor);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.DeactivateAsync(Vendor, listing.Id));
        Assert.True(listing.IsActive);
    }

    // ---------- Čtení ----------

    [Fact]
    public async Task GetById_DeactivatedListing_ThrowsNotFound()
    {
        var listing = _repo.Seed(Vendor, isActive: false);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(listing.Id));
    }

    [Fact]
    public async Task GetActive_ExcludesDeactivatedListings()
    {
        _repo.Seed(Vendor, title: "Visible");
        _repo.Seed(Vendor, title: "Hidden", isActive: false);

        var result = await _service.GetActiveAsync();

        Assert.Equal(["Visible"], result.Select(l => l.Title));
    }

    [Fact]
    public async Task GetActive_WithCategory_ReturnsOnlyThatCategory()
    {
        _repo.Seed(Vendor, title: "Pills", categoryId: 1);
        _repo.Seed(Vendor, title: "Knife", categoryId: 2);

        var result = await _service.GetActiveAsync(categoryId: 2);

        Assert.Equal(["Knife"], result.Select(l => l.Title));
    }

    [Fact]
    public async Task GetMine_ReturnsAlsoDeactivatedListingsOfVendor()
    {
        _repo.Seed(Vendor, title: "Active");
        _repo.Seed(Vendor, title: "Old", isActive: false);
        _repo.Seed(OtherVendor, title: "Not mine");

        var result = await _service.GetMineAsync(Vendor);

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, l => l.Title == "Not mine");
    }
}