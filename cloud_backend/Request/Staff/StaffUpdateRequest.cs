namespace cloud_backend.Request.Staff
{
    public class StaffUpdateRequest
    {
        public int? role { get; set; }
        public string? username { get; set; }
        public string? name { get; set; }
        public string? password { get; set; }
        public string? email { get; set; }
        public string? contactNumber { get; set; }
        public bool? isActive { get; set; }
    }
}
