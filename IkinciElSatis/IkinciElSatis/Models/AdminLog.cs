using System.ComponentModel.DataAnnotations.Schema;

namespace IkinciElSatis.Models
{
    public class AdminLog
    {
        public int Id { get; set; }

        public string? AdminId { get; set; } // İşlemi yapan Yöneticinin ID'si
        [ForeignKey("AdminId")]
        public ApplicationUser? Admin { get; set; }

        public string Action { get; set; } // İşlem Başlığı
        public string Description { get; set; } // Detay

        public string? IpAddress { get; set; } // Hangi IP'den yapıldı?
        public DateTime Date { get; set; } = DateTime.Now;
    }
}