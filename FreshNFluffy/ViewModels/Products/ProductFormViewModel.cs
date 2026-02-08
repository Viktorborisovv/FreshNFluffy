
namespace FreshNFluffy.ViewModels.Products
{
    using FreshNFluffy.ViewModels.Categories;
    using System.ComponentModel.DataAnnotations;
    using static FreshNFluffy.Common.EntityValidation.Product;
    public class ProductFormViewModel
    {
        public int? ProductId { get; set; }

        [Required]
        [StringLength(ProductNameMaxLength, MinimumLength = ProductNameMinLength)]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        [MinLength(DescriptionMinLength)]
        public string? Description { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "100000")]
        public decimal Price { get; set; }

        [MaxLength(ProductImageUrlMaxLength)]
        [Url]
        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public int NutritionTypes { get; set; }

        public IEnumerable<CategorySelectViewModel> Categories { get; set; }
        = new List<CategorySelectViewModel>();
    }
}
