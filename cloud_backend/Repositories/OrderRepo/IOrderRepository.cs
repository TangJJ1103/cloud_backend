using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Order;
using cloud_backend.Result;

namespace cloud_backend.Repositories.OrderRepo
{
    public interface IOrderRepository
    {

        Task<IEnumerable<GetOrdersDto?>> GetAllOrdersDto();
        Task<GetOrdersDto?> GetOrderDtoById(Guid orderId);
        Task<IEnumerable<GetOrdersDto?>> GetUserOrdersDto(Guid credentialId);
        Task<IEnumerable<Orders?>> GetDailyOrders();
        Task<IEnumerable<Orders?>> GetWeeklyOrders();
        Task<CreateOrderResult?> CreateOrderAsync(CreateOrderRequest request);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, int status);
    }
}
