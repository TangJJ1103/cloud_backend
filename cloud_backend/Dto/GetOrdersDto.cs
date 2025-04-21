
namespace cloud_backend.Dto
{
    public class GetOrdersDto
    {
        public Guid orderId { get; set; }
        public Guid credentialId { get; set; }

        public int quantity { get; set; }
        public double amount { get; set; }
        public int? discountPercentage { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? fulfilledAt { get; set; }
        public int status { get; set; }
        public GetReceiptOnlyDto receipt { get; set; }
        public ICollection<GetOrderItemsDto> orderItems { get; set; }
    }
}
