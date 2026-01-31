namespace FreshNFluffy.ViewModels.Orders
{
    public class AddOrderItemViewModel
    {
        public int OrderRequestId { get; set; }

        public bool IsLocked { get; set; }

        public IEnumerable<ProductSelectViewModel> Products { get; set; }
            = new List<ProductSelectViewModel>();

        public IEnumerable<OrderItemRowViewModel> CurrentItems { get; set; }
         = new List<OrderItemRowViewModel>();

        public AddOrderItemInputModel NewItem { get; set; } = new();
    }
}
