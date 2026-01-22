using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IkinciElSatis.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } 

        [Display(Name = "Açıklama")]
        public string Description { get; set; } 

        [Display(Name = "Üst Kategori")]
        public int? ParentId { get; set; } 

        [ForeignKey("ParentId")]
        public Category Parent { get; set; } 

        public ICollection<Category> SubCategories { get; set; } 

        public ICollection<Product> Products { get; set; } 
    }
}