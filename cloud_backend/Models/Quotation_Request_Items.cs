using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    public class Quotation_Request_Items
    {
        [Key]
        public Guid quotationRequestItemId { get; set; }

        [Required]
        public Guid quotationRequestId { get; set; }
        [Required]
        public Guid productId { get; set; }

        public double unitPrice { get; set; }
        public int quantity { get; set; }
        public int discountPercentage { get; set; }

        [ForeignKey(nameof(productId))]
        public virtual Products? Products { get; set; }

        [ForeignKey(nameof(quotationRequestId))]
        [JsonIgnore]
        public virtual Quotation_Request? Quotation_Request { get; set; }
    }
}
