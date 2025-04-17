namespace cloud_backend.Request.Product
{
    public class ProductUpdateRequest
    {
        public string? name { get; set; }
        public double? cost { get; set; }
        public double? price { get; set; }
        public int? stockQuantity { get; set; }
        public string? description { get; set; }
        public string? model { get; set; }
        public string? category { get; set; }
        public bool? isActive { get; set; }
        public int? discountPercentage { get; set; }
        public string? imageUrl { get; set; }
    }
}
