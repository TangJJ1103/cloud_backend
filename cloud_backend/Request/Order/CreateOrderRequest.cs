namespace cloud_backend.Request.Order
{
    public class CreateOrderRequest
    {
        public Guid credentialId { get; set; }
        public Guid? quotationId { get; set; }
        public int discountPercentage { get; set; }
        public int paymentMethod { get; set; }
        public int paymentType { get; set; }
        public List<OrderItemRequest> orderItems { get; set; }
    }
}
