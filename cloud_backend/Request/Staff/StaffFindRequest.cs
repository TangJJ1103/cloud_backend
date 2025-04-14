namespace cloud_backend.Request.Staff
{
    public class StaffFindRequest
    {
        public Guid staffId { get; set; }
        public int role { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public bool isActive { get; set; }
    }
}
