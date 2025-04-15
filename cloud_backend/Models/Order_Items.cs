using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    [Table("order_items")]
    public class Order_Items
    {
        [Key] public Guid orderItemId { get; set; }
        [Required]
        public Guid orderId { get; set; }
        [Required]
        public Guid productId { get; set; }
        
        public int quantity { get; set; }
        public double unitPrice { get; set; }
        public int? discountPercentage { get; set; }

        [ForeignKey(nameof(orderId))]
        [JsonIgnore]
        public virtual Orders? Orders { get; set; }

        [ForeignKey(nameof(productId))]
        public virtual Products? Product { get; set; }
    }
}
