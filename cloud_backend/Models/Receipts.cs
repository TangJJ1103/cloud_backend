using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    [Table("receipts")]
    public class Receipts
    {
        [Key] public Guid receiptId { get; set; }
        [Required]
        public Guid credentialId { get; set; }
        [Required]
        public Guid orderId { get; set; }

        public double amount { get; set; }
        public int paymentMethod { get; set; }
        public int? paymentType { get; set; }
        public DateTime createdAt { get; set; }

        [ForeignKey(nameof(credentialId))]
        [JsonIgnore]
        public virtual User_Credentials User_Credentials { get; set; }

        [ForeignKey(nameof(orderId))]
        public virtual Orders Orders { get; set; }
    }
}
