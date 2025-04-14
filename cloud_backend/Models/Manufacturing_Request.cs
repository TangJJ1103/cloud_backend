using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    public class Manufacturing_Request
    {
        [Key] public Guid requestId { get; set; }
        [Required]
        public Guid productId { get; set; }

        public int quantity { get; set; }
        public double cost { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public int status { get; set; }

        [ForeignKey(nameof(productId))]
        public virtual Products? Products { get; set; }
    }
}
