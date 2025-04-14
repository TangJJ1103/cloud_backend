using cloud_backend.Models;

namespace cloud_backend.Result
{
    public class CreateOrderResult
    {
        public Orders order { get; set; }
        public Receipts receipt { get; set; }
    }
}
