namespace FreshNFluffy.Data.Models
{
    using FreshNFluffy.Data.Models.Enum;
    using System.ComponentModel.DataAnnotations;

    using static FreshNFluffy.Common.EntityValidation.Category;

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public NutritionTypes NutritionTypes { get; set; }

        public virtual ICollection<Product> Products { get; set; } =
               new HashSet<Product>();
    }
}
