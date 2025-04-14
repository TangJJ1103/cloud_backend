using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cloud_backend.Models
{
    public class Quotation_Request
    {
        [Key] public Guid quotationRequestId { get; set; }
        [Required]
        public Guid storeId { get; set; }

        public int status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

        [ForeignKey(nameof(storeId))]
        [JsonIgnore]
        public virtual Store_User? Store_User { get; set; }

        public virtual ICollection<Quotation_Request_Items>? quotationRequestItems { get; set; }
    }
}
