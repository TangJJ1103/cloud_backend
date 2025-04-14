using System.ComponentModel.DataAnnotations;

namespace cloud_backend.Models
{
    public class User_Credentials
    {
        [Key] public Guid credentialId { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public DateTime? lastLogOn { get; set; }
        public int role { get; set; }

        // Navigation Properties
        public virtual ICollection<Receipts?> Receipts { get; set; }
        public virtual ICollection<Orders?> Orders { get; set; }

        public virtual Customer_User? Customer_User { get; set; }
        public virtual Store_User? Store_User { get; set; }
        public virtual Staff_User? Factory_User { get; set; }
    }
}
