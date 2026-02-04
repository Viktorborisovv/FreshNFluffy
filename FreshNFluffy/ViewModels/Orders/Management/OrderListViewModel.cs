namespace FreshNFluffy.ViewModels.Orders.Management
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderListItemViewModel> Orders { get; set; } 
            = new List<OrderListItemViewModel>();

        public int? StatusFilter { get; set; }

        public string? SearchTerm { get; set; }     
    }
}
