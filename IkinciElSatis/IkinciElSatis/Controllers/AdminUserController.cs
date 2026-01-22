using IkinciElSatis.Models;
using IkinciElSatis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IkinciElSatis.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin girebilir
    public class AdminUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. ÜYE LİSTESİ VE ARAMA
        public async Task<IActionResult> Index(string? search)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(search) ||
                                         u.FirstName.ToLower().Contains(search) ||
                                         u.LastName.ToLower().Contains(search));
            }

            var users = await query.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            ViewData["CurrentFilter"] = search;
            return View(userViewModels);
        }

        // 2. ÜYE SİLME (POST)
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id == user.Id)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        // 3. ÜYE DÜZENLEME (GET - Formu Getir)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var model = new AdminUserEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = roles.FirstOrDefault()
            };

            ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            return View(model);
        }

        // 4. ÜYE DÜZENLEME (POST - Kaydet)
        [HttpPost]
        public async Task<IActionResult> Edit(AdminUserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                // Rol güncelleme işlemi
                var currentRoles = await _userManager.GetRolesAsync(user);
                var currentRole = currentRoles.FirstOrDefault();

                if (currentRole != model.Role)
                {
                    // Eski rolü sil, yeni rolü ekle
                    if (!string.IsNullOrEmpty(currentRole))
                    {
                        await _userManager.RemoveFromRoleAsync(user, currentRole);
                    }
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                TempData["Success"] = "Kullanıcı başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View(model);
        }
    }
}