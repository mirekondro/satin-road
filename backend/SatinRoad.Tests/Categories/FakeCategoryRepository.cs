using SatinRoad.Core.Categories;
using SatinRoad.Core.Entities;

namespace SatinRoad.Tests.Categories;


public class FakeCategoryRepository : ICategoryRepository
{
    public List<Category> Items { get; } = [];

    
    public HashSet<int> CategoriesWithListings { get; } = [];

    private int _nextId = 1;

    public Task<List<Category>> GetAllAsync() => Task.FromResult(Items.ToList());

    public Task<Category?> GetByIdAsync(int id) =>
        Task.FromResult(Items.FirstOrDefault(c => c.Id == id));

    public Task<bool> NameExistsAsync(string name, int? excludeId = null) =>
        Task.FromResult(Items.Any(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Id != excludeId));

    public Task<bool> HasListingsAsync(int categoryId) =>
        Task.FromResult(CategoriesWithListings.Contains(categoryId));

    public Task<Category> AddAsync(Category category)
    {
        category.Id = _nextId++;
        Items.Add(category);
        return Task.FromResult(category);
    }

    public Task UpdateAsync(Category category) => Task.CompletedTask; 

    public Task DeleteAsync(int id)
    {
        Items.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }

    
    public Category Seed(string name)
    {
        var c = new Category { Id = _nextId++, Name = name };
        Items.Add(c);
        return c;
    }
}