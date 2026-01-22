using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IkinciElSatis.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Products = new HashSet<Product>();
            FavoriteItems = new HashSet<FavoriteItem>();
        }

        [Display(Name = "Ad")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Display(Name = "Soyad")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Display(Name = "Doğum Tarihi")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Adres")]
        [StringLength(200)]
        public string? Address { get; set; }

        // =================================
        // İLİŞKİLER (Navigation Properties)
        // =================================

        // 1. Kullanıcının Satışa Koyduğu Ürünler
        public virtual ICollection<Product> Products { get; set; }

        // 2. Kullanıcının Favorilediği Ürünler
        public virtual ICollection<FavoriteItem> FavoriteItems { get; set; }
    }
}