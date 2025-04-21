namespace cloud_backend.Dto
{
    public class GetReceiptOnlyDto
    {
        public Guid receiptId { get; set; }
        public Guid credentialId { get; set; }
        public double amount { get; set; }
        public int paymentMethod { get; set; }
        public int? paymentType { get; set; }
        public DateTime createdAt { get; set; }
    }
}
