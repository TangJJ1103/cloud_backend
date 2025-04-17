namespace cloud_backend.Request.Quotation
{
    public class CreateQuotation
    {
        public Guid storeId { get; set; }
        public int discountPercentage { get; set; }
        public List<CreateQuotationItem> quotationItems { get; set; }
    }
}
