using FreshNFluffy.ViewModels.Categories;

namespace FreshNFluffy.ViewModels.Products
{
    public class ProductQueryViewModel
    {
        public int? CategoryId { get; set; }
        public int? NutritionTypes { get; set; }
        public string? Search { get; set; }


        public IEnumerable<CategorySelectViewModel> Categories { get; set; }
        = new List<CategorySelectViewModel>();

        public IEnumerable<ProductListItemViewModel> Products { get; set; }
        = new List<ProductListItemViewModel>();
    }
}
