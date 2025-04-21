namespace cloud_backend.Request.Quotation
{
    public class QuotationPaginationRequest
    {
        public int offset { get; set; }
        public int currentIndex { get; set; }
        public int? status { get; set; }
        public string? searchTerm { get; set; }
        public Guid? storeId { get; set; }
    }
}
