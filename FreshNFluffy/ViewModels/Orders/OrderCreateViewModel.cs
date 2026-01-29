namespace FreshNFluffy.ViewModels.Orders
{
    using System.ComponentModel.DataAnnotations;
    using static FreshNFluffy.Common.EntityValidation.OrderRequest;

    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "The Customer Name field is required.")]
        [Display(Name = "Customer Name")]
        [StringLength(CustomerNameMaxLength, MinimumLength = CustomerNameMinLength)]
        public string CustomerName { get; set; } = null!;

        [Required(ErrorMessage = "The Phone number field is required.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\+359\d{9}$", ErrorMessage = "Phone number must be in format +359XXXXXXXXX.")]
        [MaxLength(PhoneNumberLength)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [Display(Name = "Pickup Date")]
        public DateTime PickupDate { get; set; }

        [StringLength(NotesMaxLength, MinimumLength = NotesMinLength)]
        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }

        public List<OrderItemCreateViewModel> Items { get; set; } = new();
    }
}
