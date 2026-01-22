using IkinciElSatis.Data;
using IkinciElSatis.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity (Üyelik) Ayarlarý
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Repository ve Servisler
builder.Services.AddScoped<IkinciElSatis.Repositories.ICategoryRepository, IkinciElSatis.Repositories.CategoryRepository>();
builder.Services.AddScoped<IkinciElSatis.Repositories.IProductRepository, IkinciElSatis.Repositories.ProductRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IkinciElSatis.Services.LogService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// === VERÝTABANI SEEDING (Admin Ekleme) ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.SeedRolesAndAdminAsync(services); 
    }
    catch (Exception ex)
    {
        Console.WriteLine("Seeding hatasý: " + ex.Message);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();