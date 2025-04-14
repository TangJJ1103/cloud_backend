using System.ComponentModel.DataAnnotations;

namespace cloud_backend.Dto
{
    public class GetQuotationRequestsDto
    {
        public Guid quotationRequestId { get; set; }
        public Guid storeId { get; set; }
        public int status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public ICollection<GetQuotationRequestItemsDto> quotationRequestItems { get; set; }
    }
}
