using cloud_backend.Data;
using cloud_backend.Models;
using cloud_backend.Repositories.OrderRepo;
using cloud_backend.Repositories.ReceiptRepo;
using cloud_backend.Repositories.UserCredentialRepo;
using cloud_backend.Request.Order;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReceiptRepository _receiptRepository;
        private readonly IUserCredentialRepository _userCredentialRepository;

        public OrderController(IOrderRepository orderRepository, IReceiptRepository receiptRepository, IUserCredentialRepository userCredentialRepository)
        {
            _orderRepository = orderRepository;
            _receiptRepository = receiptRepository;
            _userCredentialRepository = userCredentialRepository;
        }

        [Authorize]
        [HttpPost("findAll")]
        public async Task<IActionResult> GetAllOrdersPaginated([FromBody] OrderPaginationRequest request)
        {
            var orders = await _orderRepository.GetAllOrdersDtoPaginated(request);
            return Ok(orders);
        }

        [Authorize]
        [HttpGet("findOne/{orderId}")]
        public async Task<ActionResult<Orders>> GetOrderById(Guid orderId)
        {
            var order = await _orderRepository.GetOrderDtoById(orderId);
            if (order == null) return NotFound(new { message = "Order not found." });

            return Ok(order);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if ( request == null || !request.orderItems.Any())
            {
                return BadRequest(new { message = "Invalid input" });
            }

            var user = await _userCredentialRepository.IsStoreOrCustomer(request.credentialId);

            Console.WriteLine("userStatus = " + user);
            if (!user)
            {
                return BadRequest(new { message = "Invalid order credential" });
            }

            var ok = await _orderRepository.CreateOrderAsync(request);
            if (!ok) { return BadRequest(new { message = "Order place failed" }); }
            return Ok(new { message = "Order placed successfully." });
        }

        [Authorize]
        [HttpPatch("updateStatus/{orderId}")]
        public async Task<ActionResult> UpdateOrderStatus(Guid orderId, [FromBody] OrderUpdateRequest request)
        {
            if(orderId == Guid.Empty || request == null)
            {
                return BadRequest(new { message = "Invalid input" });
            }

            var success = await _orderRepository.UpdateOrderStatusAsync(orderId, request.status);
            if (!success) return NotFound(new { message = "Order not found or update failed." });

            return Ok(new { message = "Order status updated successfully." });
        }
    }
}
