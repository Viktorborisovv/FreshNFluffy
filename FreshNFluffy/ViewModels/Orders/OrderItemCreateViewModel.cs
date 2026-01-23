namespace FreshNFluffy.ViewModels.Orders
{
    using System.ComponentModel.DataAnnotations;
    public class OrderItemCreateViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
