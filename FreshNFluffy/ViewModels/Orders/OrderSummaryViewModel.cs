namespace FreshNFluffy.ViewModels.Orders
{
    public class OrderSummaryViewModel
    {
        public int OrderRequestId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime PickupDate { get; set; }
        public string? Notes { get; set; }
        public decimal TotalPrice { get; set; }

        public IEnumerable<OrderItemRowViewModel> Items { get; set; }
               = new List<OrderItemRowViewModel>();

    }
}
