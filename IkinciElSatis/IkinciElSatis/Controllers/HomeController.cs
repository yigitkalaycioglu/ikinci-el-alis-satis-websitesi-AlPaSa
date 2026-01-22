using IkinciElSatis.Models;
using IkinciElSatis.Repositories;
using IkinciElSatis.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IkinciElSatis.Controllers
{
    public class HomeController : Controller
    {
        // Artýk _context yok! Repository'ler var.
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Kategorileri Aðaç Yapýsýnda Çek (Repo'daki Tree metodu)
            var categories = await _categoryRepository.GetCategoryTreeAsync();

            // 2. Vitrin Ürünlerini Çek (Repo'da "En çok favorilenenler" olarak ayarlamýþtýk)
            var showcaseProducts = await _productRepository.GetShowcaseProductsAsync(20);

            // 3. Günün Fýrsatýný Çek (Repo'dan)
            var dealProduct = await _productRepository.GetDealOfTheDayAsync();

            // 4. Kullanýcýnýn Favorilerini Çek (Kalplerin kýrmýzý yanmasý için)
            List<int> userFavs = new List<int>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                userFavs = await _productRepository.GetUserFavoriteIdsAsync(userId);
            }

            // Modeli Doldur ve Gönder
            var model = new HomeViewModel
            {
                Categories = categories,
                ShowcaseProducts = showcaseProducts,
                DealOfTheDay = dealProduct,
                UserFavoriteIds = userFavs
            };

            return View(model);
        }
    }
}