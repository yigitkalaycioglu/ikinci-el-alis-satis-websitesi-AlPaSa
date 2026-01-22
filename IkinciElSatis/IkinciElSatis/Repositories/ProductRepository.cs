using IkinciElSatis.Data;
using IkinciElSatis.Models;
using Microsoft.EntityFrameworkCore;

namespace IkinciElSatis.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task<List<Product>> GetShowcaseProductsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .Select(p => new
                {
                    Product = p,
                    FavCount = _context.FavoriteItems.Count(f => f.ProductId == p.Id)
                })
                .OrderByDescending(x => x.FavCount)
                .ThenByDescending(x => x.Product.Id)
                .Take(count)
                .Select(x => x.Product)
                .ToListAsync();
        }

        public async Task<Product?> GetDealOfTheDayAsync()
        {
            var topFavorite = await _context.FavoriteItems
                .GroupBy(f => f.ProductId)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            Product? dealProduct = null;

            if (topFavorite != null)
            {
                dealProduct = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == topFavorite.ProductId);
            }

            if (dealProduct == null)
            {
                dealProduct = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefaultAsync();
            }

            return dealProduct;
        }

        public async Task<List<int>> GetUserFavoriteIdsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return new List<int>();

            return await _context.FavoriteItems
                .Where(f => f.UserId == userId)
                .Select(f => f.ProductId)
                .ToListAsync();
        }

        

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllWithCategoryAsync(string? search, int? categoryId)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search) ||
                                         p.Description.ToLower().Contains(search));
            }

            if (categoryId.HasValue)
            {
                var relatedCategoryIds = await GetCategoryTreeIds(categoryId.Value);
                query = query.Where(p => relatedCategoryIds.Contains(p.CategoryId));
            }

            return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // ===========================
        // YARDIMCI METOTLAR (Private)
        // ===========================

        private async Task<List<int>> GetCategoryTreeIds(int rootCategoryId)
        {
            var allCategories = await _context.Categories.ToListAsync();
            var resultIds = new List<int>();
            CollectSubIds(rootCategoryId, allCategories, resultIds);
            return resultIds;
        }

        private void CollectSubIds(int parentId, List<Category> allCategories, List<int> resultIds)
        {
            resultIds.Add(parentId);
            var children = allCategories.Where(c => c.ParentId == parentId).ToList();
            foreach (var child in children)
            {
                CollectSubIds(child.Id, allCategories, resultIds);
            }
        }
    }
}