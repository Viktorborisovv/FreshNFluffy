using FreshNFluffy.Data.Models;
using FreshNFluffy.ViewModels.Orders;
using FreshNFluffy.ViewModels.Orders.Management;

namespace FreshNFluffy.Data.Repository.Contracts
{
    public interface IOrderRepository
    {
        Task AddOrderRequestAsync(OrderRequest orderRequest);


        Task<OrderRequest?> GetOrderRequestByIdAsync(int orderRequestId);

        Task<OrderRequest?> GetOrderRequestByIdAsNoTrackingAsync(int orderRequestId);

        Task<bool> OrderRequestExistsAsync(int orderRequestId);

        Task<bool> IsOrderLockedAsync(int orderRequestId);

        Task<IEnumerable<ProductSelectViewModel>> GetProductsForOrderAsync();

        Task<IEnumerable<OrderItemRowViewModel>> GetOrderItemsByOrderRequestIdAsync(int orderRequestId);

        Task<Product?> GetProductByIdAsync(int productId);
        Task<OrderItem?> GetOrderItemByOrderAndProductAsync(int orderRequestId, int productId);

        Task AddOrderItemAsync(OrderItem orderItem);

        Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId);

        void RemoveOrderItem(OrderItem orderItem);

        Task<IEnumerable<OrderListItemViewModel>> GetOrdersForManagementAsync(int? statusFilter, string? searchTerm);
        Task<bool> UserOwnsOrderAsync(int orderRequestId, string userId);
        Task<int> SaveChangesAsync();
    }
}
