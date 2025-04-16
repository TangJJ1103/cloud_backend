using cloud_backend.Dto;
using cloud_backend.Models;
using cloud_backend.Request.Order;

namespace cloud_backend.Repositories.OrderRepo
{
    public interface IOrderRepository
    {

        Task<IEnumerable<GetOrdersDto?>> GetAllOrdersDto();
        Task<GetPaginatedDto<GetOrdersDto>> GetAllOrdersDtoPaginated(OrderPaginationRequest request);
        Task<GetOrdersDto?> GetOrderDtoById(Guid orderId);
        Task<IEnumerable<GetOrdersDto?>> GetUserOrdersDto(Guid credentialId);
        Task<IEnumerable<Orders?>> GetDailyOrders();
        Task<IEnumerable<Orders?>> GetWeeklyOrders();
        Task<bool> CreateOrderAsync(CreateOrderRequest request);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, int status);
    }
}
