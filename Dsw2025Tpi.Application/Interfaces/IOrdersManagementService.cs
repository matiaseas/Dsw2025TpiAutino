using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IOrdersManagementService
    {
        Task<OrderModel.OrderResponse> AddOrder(OrderModel.OrderRequest request);
        Task<List<OrderModel.OrderResponse>?> GetOrders();
        Task<OrderModel.OrderResponse?> GetOrderById(Guid id);
        Task<OrderModel.OrderResponse> UpdateOrderStatus(Guid id, OrderModel.OrderStatusRequest request);
    }
}