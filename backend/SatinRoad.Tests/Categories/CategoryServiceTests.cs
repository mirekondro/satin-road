using SatinRoad.Core.Categories;
using SatinRoad.Core.Common;

namespace SatinRoad.Tests.Categories;

public class CategoryServiceTests
{
    private readonly FakeCategoryRepository _repo = new();
    private readonly CategoryService _service;

    public CategoryServiceTests() => _service = new CategoryService(_repo);

    // ---------- Create ----------

    [Fact]
    public async Task Create_ValidName_SavesTrimmedName()
    {
        var created = await _service.CreateAsync("  Poisons  ");

        Assert.Equal("Poisons", created.Name);
        Assert.Single(_repo.Items);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A")] 
    public async Task Create_InvalidName_ThrowsValidation(string? name)
    {
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(name));
        Assert.Empty(_repo.Items);
    }

    [Fact]
    public async Task Create_NameLongerThan50_ThrowsValidation()
    {
        var name = new string('x', 51);
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(name));
    }

    [Fact]
    public async Task Create_NameExactly50_IsAllowed() 
    {
        var created = await _service.CreateAsync(new string('x', 50));
        Assert.Equal(50, created.Name.Length);
    }

    [Fact]
    public async Task Create_DuplicateNameIgnoringCase_ThrowsConflict()
    {
        _repo.Seed("Drugs");
        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync("dRuGs"));
    }

    // ---------- GetAll ----------

    [Fact]
    public async Task GetAll_ReturnsCategoriesSortedByName()
    {
        _repo.Seed("Weapons");
        _repo.Seed("Counterfeits");
        _repo.Seed("Drugs");

        var result = await _service.GetAllAsync();

        Assert.Equal(["Counterfeits", "Drugs", "Weapons"], result.Select(c => c.Name));
    }

    // ---------- Update ----------

    [Fact]
    public async Task Update_NonExistingId_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(999, "Anything"));
    }

    [Fact]
    public async Task Update_ValidName_RenamesCategory()
    {
        var cat = _repo.Seed("Drugs");

        var updated = await _service.UpdateAsync(cat.Id, " Medicine ");

        Assert.Equal("Medicine", updated.Name);
    }

    [Fact]
    public async Task Update_ToNameOfAnotherCategory_ThrowsConflict()
    {
        _repo.Seed("Drugs");
        var weapons = _repo.Seed("Weapons");

        await Assert.ThrowsAsync<ConflictException>(() => _service.UpdateAsync(weapons.Id, "drugs"));
    }

    [Fact]
    public async Task Update_SameNameDifferentCase_IsAllowed() 
    {
        var cat = _repo.Seed("drugs");

        var updated = await _service.UpdateAsync(cat.Id, "Drugs");

        Assert.Equal("Drugs", updated.Name);
    }

    [Fact]
    public async Task Update_InvalidName_ThrowsValidation()
    {
        var cat = _repo.Seed("Drugs");
        await Assert.ThrowsAsync<ValidationException>(() => _service.UpdateAsync(cat.Id, " "));
    }

    // ---------- Delete ----------

    [Fact]
    public async Task Delete_NonExistingId_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
    }

    [Fact]
    public async Task Delete_CategoryWithListings_ThrowsConflict()
    {
        var cat = _repo.Seed("Drugs");
        _repo.CategoriesWithListings.Add(cat.Id);

        await Assert.ThrowsAsync<ConflictException>(() => _service.DeleteAsync(cat.Id));
        Assert.Single(_repo.Items); 
    }

    [Fact]
    public async Task Delete_CategoryWithoutListings_RemovesIt()
    {
        var cat = _repo.Seed("Drugs");

        await _service.DeleteAsync(cat.Id);

        Assert.Empty(_repo.Items);
    }
}