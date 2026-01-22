using IkinciElSatis.Data;
using IkinciElSatis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IkinciElSatis.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin girebilir
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                UserCount = await _context.Users.CountAsync(),
                ProductCount = await _context.Products.CountAsync(),
                CategoryCount = await _context.Categories.CountAsync()
            };

            return View(model);
        }
    }
}