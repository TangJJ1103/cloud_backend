using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    [Table("quotation_items")]
    public class Quotation_Items
    {
        [Key] public Guid quotationItemId { get; set; }
        [Required]
        public Guid quotationId { get; set; }
        [Required]
        public Guid productId { get; set; }

        public int quantity { get; set; }
        public double unitPrice { get; set; }
        public int? discountPercentage { get; set; }

        [ForeignKey(nameof(quotationId))]
        [JsonIgnore]
        public virtual Quotations? Quotations { get; set; }

        [ForeignKey(nameof(productId))]
        public virtual Products? Products { get; set; }
    }
}
