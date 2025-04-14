using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    public class Quotations
    {
        [Key] public Guid quotationId { get; set; }
        [Required]
        public Guid storeId { get; set; }
        [Required]
        public Guid orderId { get; set; }

        public int status { get; set; }
        public int discountPercentage { get; set; }
        public double totalAmount { get; set; }
        public int totalQuantity { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

        [ForeignKey(nameof(storeId))]
        [JsonIgnore]
        public virtual Store_User? Store_User { get; set; }

        [ForeignKey(nameof(orderId))]
        [JsonIgnore]
        public virtual Orders? Orders { get; set; }

        public virtual ICollection<Quotation_Items>? quotationItems { get; set; }
    }
}
