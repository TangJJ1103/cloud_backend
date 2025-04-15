using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    [Table("customer_user")]
    public class Customer_User
    {
        [Key] public Guid customerId { get; set; }

        [Required]
        public Guid credentialId { get; set; }

        public string? address { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string? verificationToken { get; set; }
        public bool isVerified { get; set; }

        [ForeignKey(nameof(credentialId))]
        [JsonIgnore]
        public virtual User_Credentials User_Credential { get; set; } 
    }
}
