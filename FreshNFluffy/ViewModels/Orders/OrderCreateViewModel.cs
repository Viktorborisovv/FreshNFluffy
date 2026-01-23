namespace FreshNFluffy.ViewModels.Orders
{
    using System.ComponentModel.DataAnnotations;
    using static FreshNFluffy.Common.EntityValidation.OrderRequest;

    public class OrderCreateViewModel
    {
        [Required]
        [StringLength(CustomerNameMaxLength, MinimumLength = CustomerNameMinLength)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [MaxLength(PhoneNumberLength)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public DateTime PickupDate { get; set; }

        [StringLength(NotesMaxLength, MinimumLength = NotesMinLength)]
        public string? Notes { get; set; }

        public List<OrderItemCreateViewModel> Items { get; set; } = new();
    }
}
