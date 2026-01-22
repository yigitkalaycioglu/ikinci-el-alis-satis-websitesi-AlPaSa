using IkinciElSatis.Data;
using IkinciElSatis.Models;
using IkinciElSatis.Repositories;
using IkinciElSatis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IkinciElSatis.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;
        private readonly LogService _logger;

        public ProductController(IProductRepository productRepository,
                                 ICategoryRepository categoryRepository,
                                 IWebHostEnvironment webHostEnvironment,
                                 AppDbContext context,
                                 LogService logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _logger = logger;
        }

        // 1. ÜRÜN LİSTESİ (VİTRİN)
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var products = await _productRepository.GetAllWithCategoryAsync(search, categoryId);
            ViewData["CurrentFilter"] = search;

            if (categoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId.Value);
                ViewBag.CurrentCategoryName = category?.Name;
            }

            return View(products);
        }

        // 2. DETAY SAYFASI
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                bool isFavorited = await _context.FavoriteItems
                    .AnyAsync(f => f.UserId == userId && f.ProductId == id);

                ViewBag.IsFavorited = isFavorited;
            }

            return View(product);
        }

        // 3. EKLEME (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var rawCategories = await _context.Categories.Include(c => c.Parent).ToListAsync();
            var selectListItems = rawCategories.Select(c => new
            {
                Id = c.Id,
                Name = c.Parent != null ? $"{c.Parent.Name} > {c.Name}" : c.Name
            }).OrderBy(c => c.Name);

            ViewBag.Categories = new SelectList(selectListItems, "Id", "Name");
            return View();
        }

        // 4. EKLEME (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ModelState.Remove("User");
            ModelState.Remove("Category");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "products");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImagePath = uniqueFileName;
                }

                product.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                product.CreatedDate = DateTime.Now;

                await _productRepository.AddAsync(product);

                await _logger.LogAsync("Ürün Eklendi", $"'{product.Name}' isimli yeni bir ürün satışa çıkarıldı.");

                return RedirectToAction(nameof(Index));
            }

            var rawCategories = await _context.Categories.Include(c => c.Parent).ToListAsync();
            var selectListItems = rawCategories.Select(c => new
            {
                Id = c.Id,
                Name = c.Parent != null ? $"{c.Parent.Name} > {c.Name}" : c.Name
            }).OrderBy(c => c.Name);
            ViewBag.Categories = new SelectList(selectListItems, "Id", "Name");

            return View(product);
        }

        // 5. DÜZENLEME (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var rawCategories = await _context.Categories.Include(c => c.Parent).ToListAsync();
            var selectListItems = rawCategories.Select(c => new
            {
                Id = c.Id,
                Name = c.Parent != null ? $"{c.Parent.Name} > {c.Name}" : c.Name
            }).OrderBy(c => c.Name);

            ViewBag.Categories = new SelectList(selectListItems, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // 6. DÜZENLEME (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            var existingProduct = await _productRepository.GetByIdAsync(product.Id);
            if (existingProduct == null) return NotFound();

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingProduct.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            List<string> changes = new List<string>();

            if (existingProduct.Name != product.Name) changes.Add($"Ad: '{existingProduct.Name}' -> '{product.Name}'");
            if (existingProduct.Price != product.Price) changes.Add($"Fiyat: {existingProduct.Price:C2} -> {product.Price:C2}");
            if (existingProduct.Description != product.Description) changes.Add("Açıklama değişti");

            if (existingProduct.CategoryId != product.CategoryId)
            {
                var oldCategoryName = existingProduct.Category?.Name ?? "Eski";
                changes.Add($"Kategori değişti (Eski: {oldCategoryName})");
            }

            if (product.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "products");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }
                existingProduct.ImagePath = uniqueFileName;
                changes.Add("Resim güncellendi");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;

            await _productRepository.UpdateAsync(existingProduct);

            if (changes.Any())
            {
                string changeLog = string.Join(", ", changes);
                await _logger.LogAsync("Ürün Düzenlendi", $"'{product.Name}' (ID: {product.Id}) değişiklikler: {changeLog}");
            }

            return RedirectToAction(nameof(Index));
        }

        // 7. SİLME (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.UserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            return View(product);
        }

        // 8. SİLME (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.UserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            string deletedProductName = product.Name;
            await _productRepository.DeleteAsync(id);

            await _logger.LogAsync("Ürün Silindi", $"'{deletedProductName}' (ID: {id}) silindi.");

            return RedirectToAction(nameof(Index));
        }
    }
}