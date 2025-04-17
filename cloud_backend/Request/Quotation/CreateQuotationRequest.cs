using cloud_backend.Request.Order;

namespace cloud_backend.Request.Quotation
{
    public class CreateQuotationRequest
    {
        public Guid storeId { get; set; }
        public List<CreateQuotationRequestItem> quotationRequestItems { get; set; }
    }
}
