namespace cloud_backend.Request.Quotation
{
    public class CreateQuotationItem
    {
        public Guid productId { get; set; }
        public double unitPrice { get; set; }
        public int quantity { get; set; }
        public int discountPercentage { get; set; }
    }
}
