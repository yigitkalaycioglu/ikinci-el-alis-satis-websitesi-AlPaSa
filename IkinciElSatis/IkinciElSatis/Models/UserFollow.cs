using System.ComponentModel.DataAnnotations.Schema;

namespace IkinciElSatis.Models
{
    public class UserFollow
    {
        public int Id { get; set; }

        public string FollowerId { get; set; }
        [ForeignKey("FollowerId")]
        public ApplicationUser? Follower { get; set; }

        public string FolloweeId { get; set; }
        [ForeignKey("FolloweeId")]
        public ApplicationUser? Followee { get; set; }

        public DateTime FollowDate { get; set; } = DateTime.Now;
    }
}