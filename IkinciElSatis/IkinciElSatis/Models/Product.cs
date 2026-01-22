using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IkinciElSatis.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Ürün Adı")]
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; }

        [Display(Name = "Fiyat")]
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Display(Name = "Resim")]
        public string? ImagePath { get; set; }

        [NotMapped]
        [Display(Name = "Resim Yükle")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Satıldı mı?")]
        public bool IsSold { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}