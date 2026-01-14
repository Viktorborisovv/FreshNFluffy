namespace FreshNFluffy.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static FreshNFluffy.Common.EntityValidation.Product;
    public class Product
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(ProductNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(ProductNameMaxLength)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = PriceSqlType)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;
    }
}
