namespace cloud_backend.Dto
{
    public class GetQuotationItemsDto
    {
        public Guid quotationItemId { get; set; }
        public Guid quotationId { get; set; }
        public Guid productId { get; set; }
        public int quantity { get; set; }
        public double unitPrice { get; set; }
        public int? discountPercentage { get; set; }
        public GetProductsDto product { get; set; }
    }
}
