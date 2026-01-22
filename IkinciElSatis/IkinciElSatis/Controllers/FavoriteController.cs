using IkinciElSatis.Data;
using IkinciElSatis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IkinciElSatis.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly AppDbContext _context;

        public FavoriteController(AppDbContext context)
        {
            _context = context;
        }

        // 1. FAVORİLERİM SAYFASI
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var favorites = await _context.FavoriteItems
                .Include(f => f.Product)
                .ThenInclude(p => p.User)
                .Include(f => f.Product.Category)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();

            return View(favorites);
        }

        // 2. TOGGLE (AJAX İLE EKLE/ÇIKAR)
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var existingFav = await _context.FavoriteItems
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == id);

                if (existingFav != null)
                {
                    _context.FavoriteItems.Remove(existingFav);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, status = "removed" });
                }
                else
                {
                    var newFav = new FavoriteItem
                    {
                        UserId = userId,
                        ProductId = id,
                        CreatedDate = DateTime.Now
                    };
                    _context.FavoriteItems.Add(newFav);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, status = "added" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 3. LİSTEDEN SİL (SAYFA ÜZERİNDEN)
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var fav = await _context.FavoriteItems
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (fav != null)
            {
                _context.FavoriteItems.Remove(fav);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün favorilerinizden kaldırıldı.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}