using IkinciElSatis.Models;

namespace IkinciElSatis.ViewModels
{
    public class UserProfileViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; }  
        public DateTime? BirthDate { get; set; } 
        public string Address { get; set; }   
        public List<Product> UserProducts { get; set; }
    }
}