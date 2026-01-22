using IkinciElSatis.Models;

namespace IkinciElSatis.ViewModels
{
    public class ChatViewModel
    {
        public List<ApplicationUser> Conversations { get; set; } = new List<ApplicationUser>();

        public List<Message> Messages { get; set; } = new List<Message>();

        public string? CurrentReceiverId { get; set; }
        public ApplicationUser? CurrentReceiver { get; set; }

        public string? RelatedProductName { get; set; }
    }
}