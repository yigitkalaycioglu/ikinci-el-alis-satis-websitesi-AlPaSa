using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IkinciElSatis.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public ApplicationUser? Sender { get; set; }

        public string? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public ApplicationUser? Receiver { get; set; }

        public string? RelatedProductName { get; set; }

        [Required(ErrorMessage = "Mesaj içeriği boş olamaz.")]
        [StringLength(1000, ErrorMessage = "Mesaj çok uzun.")]
        public string Content { get; set; }

        public DateTime SentDate { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
    }
}