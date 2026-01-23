using System.ComponentModel.DataAnnotations;

namespace FreshNFluffy.ViewModels.Orders
{
    public class AddOrderItemInputModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a product")]
        public int OrderRequestId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
