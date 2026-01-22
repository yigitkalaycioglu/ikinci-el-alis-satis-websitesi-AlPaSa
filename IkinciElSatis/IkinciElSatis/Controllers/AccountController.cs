using IkinciElSatis.Models;
using IkinciElSatis.Repositories;
using IkinciElSatis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IkinciElSatis.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IProductRepository _productRepository;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 IProductRepository productRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _productRepository = productRepository;
        }

        // === GİRİŞ YAP ===
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Email veya şifre hatalı.");
            return View(model);
        }

        // === KAYIT OL ===
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName, 
                LastName = model.LastName,   
                BirthDate = model.BirthDate, 
                Address = model.Address      
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // === ÇIKIŞ YAP ===
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // === PROFİL SAYFASI ===
        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

            var myProducts = await _productRepository.GetAllByUserIdAsync(userId);

            var model = new UserProfileViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName ?? "Belirtilmemiş",
                LastName = user.LastName ?? "Belirtilmemiş",
                BirthDate = user.BirthDate,
                Address = user.Address ?? "Belirtilmemiş",
                UserProducts = myProducts
            };

            return View(model);
        }

        // === 1. PROFİL DÜZENLEME (GET - Formu Getir) ===
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new UserEditViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Address = user.Address
            };

            return View(model);
        }

        // === 2. PROFİL DÜZENLEME (POST - Kaydet) ===
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Verileri güncelle
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.BirthDate = model.BirthDate;
            user.Address = model.Address;

            // Veritabanına kaydet
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("MyProfile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // === 3. HESAP SİLME (POST) ===
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // 1. Önce çıkış yaptır
            await _signInManager.SignOutAsync();

            // 2. Kullanıcıyı sil
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("MyProfile");
        }
    }
}