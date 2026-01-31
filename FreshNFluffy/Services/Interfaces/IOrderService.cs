namespace FreshNFluffy.Services.Interfaces
{
    using FreshNFluffy.Data.Models.Enum;

    using FreshNFluffy.ViewModels.Orders;
    using FreshNFluffy.ViewModels.Orders.Management;

    public interface IOrderService
    {
        Task<int> CreateOrderAsync(OrderCreateViewModel model);

        Task<AddOrderItemViewModel?> GetAddItemsFormAsync(int orderRequestId);
        Task<bool> AddItemAsync(AddOrderItemInputModel model);

        Task<OrderSummaryViewModel?> GetSummaryAsync(int orderRequestId);

        Task<bool> UpdateItemQuantityAsync(int orderItemId, int newQuantity);

        Task<bool> RemoveItemAsync(int orderItemId);

        Task<OrderListViewModel> GetAllForManagementAsync();

        Task<bool> UpdateStatusAsync(int orderRequestId, OrderStatus newStatus);

        Task<OrderDetailsViewModel?> GetDetailsForManagementAsync(int orderRequestId);
    }
}
