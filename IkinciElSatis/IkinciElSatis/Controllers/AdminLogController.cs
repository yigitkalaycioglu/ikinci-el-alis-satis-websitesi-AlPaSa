using IkinciElSatis.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IkinciElSatis.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminLogController : Controller
    {
        private readonly AppDbContext _context;

        public AdminLogController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // En son yapılan işlem en üstte görünsün (OrderByDescending)
            var logs = await _context.AdminLogs
                .Include(l => l.Admin)
                .OrderByDescending(l => l.Date)
                .Take(100)
                .ToListAsync();

            return View(logs);
        }
    }
}