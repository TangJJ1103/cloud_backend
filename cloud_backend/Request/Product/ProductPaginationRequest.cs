namespace cloud_backend.Request.Product
{
    public class ProductPaginationRequest
    {
        public int offset { get; set; }
        public int currentIndex { get; set; }
        public bool? isActive { get; set; }
        public string? searchTerm { get; set; }
        public Guid? credentialId { get; set; }
    }
}
