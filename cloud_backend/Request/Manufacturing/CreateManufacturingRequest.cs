namespace cloud_backend.Request.Manufacturing
{
    public class CreateManufacturingRequest
    {
        public Guid productId { get; set; }
        public int quantity { get; set; }
        public double cost { get; set; }
    }
}
