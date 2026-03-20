using FreshNFluffy.ViewModels.Categories;

namespace FreshNFluffy.ViewModels.Products
{
    public class ProductQueryViewModel
    {
        public int? CategoryId { get; set; }
        public int? NutritionTypes { get; set; }
        public string? Search { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int ProductsPerPage { get; set; } = 6;
        public int TotalProducts { get; set; }
 

        public IEnumerable<CategorySelectViewModel> Categories { get; set; }
        = new List<CategorySelectViewModel>();

        public IEnumerable<ProductListItemViewModel> Products { get; set; }
        = new List<ProductListItemViewModel>();
    }
}
