
namespace cloud_backend.Dto
{
    public class GetQuotationRequestItemsDto
    {
        public Guid quotationRequestItemId { get; set; }
        public Guid quotationRequestId { get; set; }
        public Guid productId { get; set; }
        public double unitPrice { get; set; }
        public int quantity { get; set; }
        public int discountPercentage { get; set; }
        public GetProductsDto product {  get; set; }
    }
}
