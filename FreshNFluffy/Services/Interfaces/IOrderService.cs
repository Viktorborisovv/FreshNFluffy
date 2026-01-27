using FreshNFluffy.ViewModels.Orders;

namespace FreshNFluffy.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(OrderCreateViewModel model);

        Task<AddOrderItemViewModel?> GetAddItemsFormAsync(int orderRequestId);
        Task<bool> AddItemAsync(AddOrderItemInputModel model);

        Task<OrderSummaryViewModel?> GetSummaryAsync(int orderRequestId);

        Task<bool> UpdateItemQuantityAsync(int orderItemId, int newQuantity);

        Task<bool> RemoveItemAsync(int orderItemId);
    }
}
