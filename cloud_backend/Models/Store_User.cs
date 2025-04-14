using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    public class Store_User
    {
        [Key] public Guid storeId { get; set; }

        [Required]
        public Guid credentialId { get; set; }

         

        public string? address { get; set; }
        public bool isActive { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        
        [ForeignKey(nameof(credentialId))]
        [JsonIgnore]
        public virtual User_Credentials User_Credential { get; set; } 

        public virtual ICollection<Quotations> Quotations { get; set; }
        public virtual ICollection<Quotation_Request> Quotation_Requests { get; set; }
    }
}
