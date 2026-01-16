namespace FreshNFluffy.Data.Models
{
    using FreshNFluffy.Data.Models.Enum;
    using System.ComponentModel.DataAnnotations;

    using static FreshNFluffy.Common.EntityValidation.OrderRequest;
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

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public virtual ICollection<OrderItem> Items { get; set; } =
            new HashSet<OrderItem>();
    }
}
