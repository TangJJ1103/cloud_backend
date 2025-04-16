namespace cloud_backend.Request.Store
{
    public class StorePaginationRequest
    {
        public int offset { get; set; }
        public int currentIndex { get; set; }
        public bool? isActive { get; set; }
        public string? searchTerm { get; set; }
    }
}
