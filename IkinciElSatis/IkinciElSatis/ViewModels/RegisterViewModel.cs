using System.ComponentModel.DataAnnotations;

namespace IkinciElSatis.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string FirstName { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; }

        [Display(Name = "Email Adresi")]
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Adres / İletişim")]
        [Required(ErrorMessage = "Adres bilgisi zorunludur.")]
        public string Address { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Şifre Tekrar")]
        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}