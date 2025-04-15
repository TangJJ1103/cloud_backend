using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    [Table("products")]
    public class Products
    {
        [Key] public Guid productId { get; set; }
        public string name { get; set; }
        public double cost { get; set; }
        public double price { get; set; }
        public int stockQuantity { get; set; }
        public string? description { get; set; }
        public string? model { get; set; }
        public string category { get; set; }
        public int discountPercentage { get; set; }
        public bool isActive { get; set; }
        public int soldQuantity { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

        [JsonIgnore]
        public virtual Order_Items? Order_Items { get; set; }
        [JsonIgnore]
        public virtual Manufacturing_Request? Manufacturing_Request { get; set; }
        [JsonIgnore]
        public virtual Quotation_Items? Quotation_Items { get; set; }
        [JsonIgnore]
        public virtual Quotation_Request_Items? Quotation_Request_Items { get; set; }
    }
}
