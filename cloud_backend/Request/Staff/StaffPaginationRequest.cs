namespace cloud_backend.Request.Staff
{
    public class StaffPaginationRequest
    {
        public int offset { get; set; }
        public int currentIndex { get; set; }
        public int? role { get; set; }
        public bool? isActive { get; set; }
        public string? filterBy { get; set; }
    }
}
