using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Categories;

public class CategoryService(ICategoryRepository repo)
{
    public Task<List<Category>> GetAllAsync() => throw new NotImplementedException();

    public Task<Category> CreateAsync(string? name) => throw new NotImplementedException();

    public Task<Category> UpdateAsync(int id, string? name) => throw new NotImplementedException();

    public Task DeleteAsync(int id) => throw new NotImplementedException();
}