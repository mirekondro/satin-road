using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Categories;

public class CategoryService(ICategoryRepository repo)
{
    public const int MinNameLength = 2;
    public const int MaxNameLength = 50;

    public async Task<List<Category>> GetAllAsync()
    {
        var categories = await repo.GetAllAsync();
        return categories.OrderBy(c => c.Name).ToList();
    }

    public async Task<Category> CreateAsync(string? name)
    {
        var cleanName = ValidateName(name);

        if (await repo.NameExistsAsync(cleanName))
            throw new ConflictException($"Category '{cleanName}' already exists.");

        return await repo.AddAsync(new Category { Name = cleanName });
    }

    public async Task<Category> UpdateAsync(int id, string? name)
    {
        var category = await repo.GetByIdAsync(id)
                       ?? throw new NotFoundException($"Category {id} not found.");

        var cleanName = ValidateName(name);

        // excludeId: id → kategorie nekoliduje sama se sebou (drugs → Drugs je OK)
        if (await repo.NameExistsAsync(cleanName, excludeId: id))
            throw new ConflictException($"Category '{cleanName}' already exists.");

        category.Name = cleanName;
        await repo.UpdateAsync(category);
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        _ = await repo.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category {id} not found.");

        if (await repo.HasListingsAsync(id))
            throw new ConflictException("Cannot delete a category that still has listings.");

        await repo.DeleteAsync(id);
    }
    
    private static string ValidateName(string? name)
    {
        var trimmed = name?.Trim() ?? "";

        if (trimmed.Length < MinNameLength || trimmed.Length > MaxNameLength)
            throw new ValidationException(
                $"Category name must be {MinNameLength}-{MaxNameLength} characters long.");

        return trimmed;
    }
}