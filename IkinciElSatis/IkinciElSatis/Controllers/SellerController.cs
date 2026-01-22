using IkinciElSatis.Data;
using IkinciElSatis.Models;
using IkinciElSatis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IkinciElSatis.Controllers
{
    public class SellerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SellerController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. SATICI PROFİL SAYFASI
        public async Task<IActionResult> Profile(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var seller = await _userManager.FindByIdAsync(id);
            if (seller == null) return NotFound();

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var followerCount = await _context.UserFollows.CountAsync(f => f.FolloweeId == id);

            bool isFollowing = false;
            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                isFollowing = await _context.UserFollows
                    .AnyAsync(f => f.FollowerId == currentUserId && f.FolloweeId == id);
            }

            var model = new SellerProfileViewModel
            {
                Seller = seller,
                Products = products,
                FollowerCount = followerCount,
                IsFollowing = isFollowing
            };

            return View(model);
        }

        // 2. TAKİP ET / BIRAK (Toggle)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleFollow(string followeeId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == followeeId)
                return RedirectToAction("Profile", new { id = followeeId });

            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FolloweeId == followeeId);

            if (existingFollow != null)
            {
                _context.UserFollows.Remove(existingFollow);
            }
            else
            {
                var newFollow = new UserFollow
                {
                    FollowerId = currentUserId,
                    FolloweeId = followeeId
                };
                _context.UserFollows.Add(newFollow);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", new { id = followeeId });
        }
    }
}