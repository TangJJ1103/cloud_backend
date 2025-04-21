namespace cloud_backend.Dto
{
    public class GetProductsDto
    {
        public Guid productId { get; set; }
        public string name { get; set; }
        public double cost { get; set; }
        public double price { get; set; }
        public int stockQuantity { get; set; }
        public string? description { get; set; }
        public string model { get; set; }
        public string category { get; set; }
        public bool isActive { get; set; }
        public int discountPercentage { get; set; }
        public int soldQuantity { get; set; }
        public string? imageUrl { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
