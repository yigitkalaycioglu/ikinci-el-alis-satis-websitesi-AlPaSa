using IkinciElSatis.Models;

namespace IkinciElSatis.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<List<Product>> GetAllByUserIdAsync(string userId);

        Task<List<Product>> GetAllWithCategoryAsync(string? search, int? categoryId);

        Task<List<Product>> GetShowcaseProductsAsync(int count);
        Task<Product?> GetDealOfTheDayAsync();
        Task<List<int>> GetUserFavoriteIdsAsync(string userId);
    }
}