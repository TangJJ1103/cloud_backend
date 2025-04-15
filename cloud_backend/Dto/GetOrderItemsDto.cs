using cloud_backend.Models;
using System.ComponentModel.DataAnnotations;

namespace cloud_backend.Dto
{
    public class GetOrderItemsDto
    {
        public Guid orderItemId { get; set; }
        public Guid orderId { get; set; }
        public Guid productId { get; set; }
        public int quantity { get; set; }
        public double unitPrice { get; set; }
        public int? discountPercentage { get; set; }
        public GetProductsDto product { get; set; }
    }
}
