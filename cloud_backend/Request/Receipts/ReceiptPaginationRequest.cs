namespace cloud_backend.Request.Receipts
{
    public class ReceiptPaginationRequest
    {
        public int offset { get; set; }
        public int currentIndex { get; set; }
        public int? paymentMethod { get; set; }
        public int? paymentType { get; set; }
        public string? searchTerm { get; set; }
        public Guid? credentialId { get; set; }
    }
}
