using System.ComponentModel.DataAnnotations;

namespace IkinciElSatis.ViewModels
{
    public class AdminUserEditViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Ad")]
        public string? FirstName { get; set; }

        [Display(Name = "Soyad")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Rol")]
        public string? Role { get; set; }
    }
}