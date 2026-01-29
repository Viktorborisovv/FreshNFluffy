namespace FreshNFluffy.ViewModels.Orders.Management
{
    public class OrderListItemViewModel
    {
        public int OrderRequestId { get; set; }
        public string CustomerName { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public DateTime PickupDate { get; set; }

        public string Status { get; set; } = null!;

        public int StatusValue { get; set; }

        public decimal TotalPrice { get; set; } 
    }
}
