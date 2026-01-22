using IkinciElSatis.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IkinciElSatis.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            // GetService yerine GetRequiredService kullanmak daha güvenlidir
            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Rolleri Kontrol Et ve Yoksa Oluştur (Hata almamak için)
            await CreateRoleIfNotExists(roleManager, "Admin");
            await CreateRoleIfNotExists(roleManager, "User");

            // 2. Admin Kullanıcısını Kontrol Et
            var adminEmail = "admin@sakarya.edu.tr";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Sistem",
                    LastName = "Yöneticisi",
                    Address = "Sakarya Üniversitesi",
                    BirthDate = DateTime.Now,
                    EmailConfirmed = true,
                };

                // Kullanıcıyı oluştur
                var result = await userManager.CreateAsync(newAdmin, "Sau.123!");

                // Başarılıysa Admin rolü ver
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }

        // Yardımcı Metot: Kod tekrarını önler
        private static async Task CreateRoleIfNotExists(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}