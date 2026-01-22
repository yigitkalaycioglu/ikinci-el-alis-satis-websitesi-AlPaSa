using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IkinciElSatis.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }
    }
}