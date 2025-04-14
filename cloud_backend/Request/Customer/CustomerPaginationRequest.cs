namespace cloud_backend.Request.Customer
{
    public class CustomerPaginationRequest
    {
        public int offset { get; set; }
        public int currentIndex { get; set; }
        public bool? isVerified { get; set; }
        public string? filterBy { get; set; }
    }
}
