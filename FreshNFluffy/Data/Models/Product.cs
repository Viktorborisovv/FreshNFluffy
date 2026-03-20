namespace FreshNFluffy.Data.Models
{
    using FreshNFluffy.Data.Models.Enum;

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static FreshNFluffy.Common.EntityValidation.Product;
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(ProductNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = PriceSqlType)]
        public decimal Price { get; set; }

        [MaxLength(ProductImageUrlMaxLength)]
        public string? ImageUrl { get; set; }

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;

        public NutritionTypes NutritionTypes { get; set; } = NutritionTypes.None;

        public virtual ICollection<Review> Reviews { get; set; } =
            new HashSet<Review>();
    }
}
