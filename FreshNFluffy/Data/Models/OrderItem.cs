namespace FreshNFluffy.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static FreshNFluffy.Common.EntityValidation.OrderItem;

    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        [ForeignKey(nameof(OrderRequest))]
        public int OrderRequestId { get; set; }
        public virtual OrderRequest OrderRequest { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = UnitPriceSqlType)]
        public decimal UnitPrice { get; set; }
    }
}
