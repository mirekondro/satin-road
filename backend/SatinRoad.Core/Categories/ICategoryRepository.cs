using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Categories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task<bool> HasListingsAsync(int categoryId);
    
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int Id); 
}