using IkinciElSatis.Models;

namespace IkinciElSatis.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> ShowcaseProducts { get; set; }
        public Product DealOfTheDay { get; set; }
        public List<int> UserFavoriteIds { get; set; } = new List<int>();
    }
}