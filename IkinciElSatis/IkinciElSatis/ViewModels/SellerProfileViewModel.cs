using IkinciElSatis.Models;

namespace IkinciElSatis.ViewModels
{
    public class SellerProfileViewModel
    {
        public ApplicationUser Seller { get; set; }
        public List<Product> Products { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowing { get; set; }
    }
}