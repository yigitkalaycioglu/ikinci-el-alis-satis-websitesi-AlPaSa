using System.ComponentModel.DataAnnotations;

namespace IkinciElSatis.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Email Adresi")]
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}