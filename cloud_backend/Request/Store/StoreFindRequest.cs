namespace cloud_backend.Request.Store
{
    public class StoreFindRequest
    {
        public Guid storeId { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string? address { get; set; }
        public string contactNumber { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public bool isActive { get; set; }
    }
}
