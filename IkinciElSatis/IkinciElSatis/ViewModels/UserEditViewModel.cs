using System.ComponentModel.DataAnnotations;

namespace IkinciElSatis.ViewModels
{
    public class UserEditViewModel
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string FirstName { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Adres / İletişim")]
        public string Address { get; set; }
    }
}