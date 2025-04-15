using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    [Table("orders")]
    public class Orders
    {
        [Key] public Guid orderId { get; set; }
        [Required]
        public Guid credentialId { get; set; }

        public int quantity { get; set; }
        public double amount { get; set; }
        public int? discountPercentage { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? fulfilledAt { get; set; }
        public int status { get; set; }

        [ForeignKey(nameof(credentialId))]
        public virtual User_Credentials? User_Credential { get; set; }
        public virtual Quotations? Quotations { get; set; }
        public virtual Receipts? Receipts { get; set; }
        public virtual ICollection<Order_Items>? OrderItems { get; set; }
    }
}
