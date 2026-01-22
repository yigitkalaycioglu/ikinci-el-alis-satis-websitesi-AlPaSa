using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IkinciElSatis.Models
{
    public class FavoriteList
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } 

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public List<FavoriteItem> Items { get; set; } = new List<FavoriteItem>();
    }
}