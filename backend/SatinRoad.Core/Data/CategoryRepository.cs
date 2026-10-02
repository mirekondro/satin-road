using LinqToDB;
using LinqToDB.Async;
using SatinRoad.Core.Categories;
using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Data;

public class CategoryRepository(AppDataConnection db) : ICategoryRepository
{
    public Task<List<Category>> GetAllAsync() =>
        db.Categories.ToListAsync();

    public Task<Category?> GetByIdAsync(int id) =>
        db.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var lower = name.ToLower();
        var query = db.Categories.Where(c => c.Name.ToLower() == lower);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return query.AnyAsync();
    }

    public Task<bool> HasListingsAsync(int categoryId) =>
        db.Listings.AnyAsync(l => l.CategoryId == categoryId);

    public async Task<Category> AddAsync(Category category)
    {
        category.Id = await db.InsertWithInt32IdentityAsync(category);
        return category;
    }

    public Task UpdateAsync(Category category) =>
        db.UpdateAsync(category);

    public Task DeleteAsync(int id) =>
        db.Categories.Where(c => c.Id == id).DeleteAsync();
}