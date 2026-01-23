namespace FreshNFluffy.ViewModels.Orders
{
    public class OrderItemRowViewModel
    {
        public int OrderItemId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal RowTotal => UnitPrice * Quantity;
    }
}
