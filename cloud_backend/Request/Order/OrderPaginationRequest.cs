namespace cloud_backend.Request.Order
{
    public class OrderPaginationRequest
    {
        public int offset { get; set; }
        public int currentIndex { get; set; }
        public int? status { get; set; }
        public string? searchTerm { get; set; }
        public Guid? credentialId { get; set; }
    }
}
