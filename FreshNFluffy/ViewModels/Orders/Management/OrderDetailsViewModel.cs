namespace FreshNFluffy.ViewModels.Orders.Management
{
    public class OrderDetailsViewModel
    {
        public int OrderRequestId { get; set; }

        public string CustomerName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
        public DateTime PickupDate { get; set; }

        public int StatusValue { get; set; }
        public string Status { get; set; } = null!;

        public string? Notes { get; set; }

        public IEnumerable<OrderItemRowViewModel> Items { get; set; }
                = new List<OrderItemRowViewModel>();

        public decimal TotalPrice { get; set; }


    }
}
