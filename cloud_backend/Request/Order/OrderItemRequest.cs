namespace cloud_backend.Request.Order
{
    public class OrderItemRequest
    {
        public Guid productId { get; set; }
        public int quantity { get; set; }
        public double unitPrice { get; set; }
        public int discountPercentage { get; set; }
    }
}
