using IkinciElSatis.Data;
using IkinciElSatis.Models;
using IkinciElSatis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IkinciElSatis.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LogService _logger;

        public CategoryController(AppDbContext context, LogService logger)
        {
            _context = context;
            _logger = logger;
        }


        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            var allCategories = await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.Products)
                .ToListAsync();

            var rootCategories = allCategories.Where(c => c.ParentId == null).ToList();

            return View(rootCategories);
        }

        // 1. ADMİN LİSTELEME (Ağaç Yapısı İçin Veri)
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // 2. EKLEME (GET)
        public IActionResult Create()
        {
            CreateCategoryDropdown();
            return View();
        }

        // 3. EKLEME (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                await _logger.LogAsync("Kategori Eklendi", $"'{category.Name}' isimli yeni kategori oluşturuldu.");
                return RedirectToAction(nameof(Index));
            }
            CreateCategoryDropdown();
            return View(category);
        }

        // 4. DÜZENLEME (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            CreateCategoryDropdown(id);
            return View(category);
        }

        // 5. DÜZENLEME (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentId == category.Id)
                {
                    ModelState.AddModelError("", "Bir kategori kendi kendisinin üst kategorisi olamaz.");
                    CreateCategoryDropdown(category.Id);
                    return View(category);
                }
                _context.Update(category);
                await _context.SaveChangesAsync();
                await _logger.LogAsync("Kategori Güncellendi", $"'{category.Name}' isimli kategori düzenlendi.");
                return RedirectToAction(nameof(Index));
            }
            CreateCategoryDropdown(category.Id);
            return View(category);
        }

        // 6. SİLME
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                var hasSub = await _context.Categories.AnyAsync(c => c.ParentId == id);
                if (hasSub)
                {
                    TempData["Error"] = "Bu kategorinin alt kategorileri var! Önce onları silmelisiniz.";
                    return RedirectToAction("Index");
                }
                var hasProduct = await _context.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProduct)
                {
                    TempData["Error"] = "Bu kategoriye ait ürünler var! Silinemez.";
                    return RedirectToAction("Index");
                }
                string deletedCategoryName = category.Name;
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                await _logger.LogAsync("Kategori Silindi", $"'{deletedCategoryName}' (ID: {id}) isimli kategori silindi.");
            }
            return RedirectToAction(nameof(Index));
        }

        // YARDIMCI METOTLAR
        private void CreateCategoryDropdown(int? ignoreId = null)
        {
            var allCategories = _context.Categories.ToList();
            var sortedList = new List<SelectListItem>();
            foreach (var cat in allCategories.Where(c => c.ParentId == null))
            {
                if (ignoreId.HasValue && cat.Id == ignoreId.Value) continue;
                AddCategoryToSelect(cat, allCategories, sortedList, 0, ignoreId);
            }
            ViewBag.MainCategories = new SelectList(sortedList, "Value", "Text");
        }

        private void AddCategoryToSelect(Category cat, List<Category> allCats, List<SelectListItem> list, int level, int? ignoreId)
        {
            string prefix = new string('-', level * 2);
            list.Add(new SelectListItem { Value = cat.Id.ToString(), Text = prefix + (level > 0 ? " " : "") + cat.Name });
            var children = allCats.Where(c => c.ParentId == cat.Id).ToList();
            foreach (var child in children)
            {
                if (ignoreId.HasValue && child.Id == ignoreId.Value) continue;
                AddCategoryToSelect(child, allCats, list, level + 1, ignoreId);
            }
        }
    }
}