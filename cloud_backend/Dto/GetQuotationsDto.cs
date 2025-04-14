using System.ComponentModel.DataAnnotations;

namespace cloud_backend.Dto
{
    public class GetQuotationsDto
    {
        public Guid quotationId { get; set; }
        public Guid storeId { get; set; }
        public Guid orderId { get; set; }
        public int status { get; set; }
        public int discountPercentage { get; set; }
        public double totalAmount { get; set; }
        public int totalQuantity { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public ICollection<GetQuotationItemsDto> quotationItems { get; set; }
    }
}
