using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    public class Staff_User
    {
        [Key] public Guid staffId { get; set; }

        [Required]
        public Guid credentialId { get; set; }

        public bool isActive { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

        [ForeignKey(nameof(credentialId))]
        [JsonIgnore]
        public virtual User_Credentials User_Credential { get; set; }  
        
    }
}
