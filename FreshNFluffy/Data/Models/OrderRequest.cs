namespace FreshNFluffy.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidation.OrderRequest;
    public class OrderRequest
    {
        public int OrderRequestId { get; set; }

        [Required]
        [MaxLength(CustomerNameMaxLength)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [MaxLength(PhoneNumberLength)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public DateTime PickupDate { get; set; }

        [MaxLength(NotesMaxLength)]
        public string? Notes { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
