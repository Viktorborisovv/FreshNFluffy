namespace FreshNFluffy.ViewModels.Products
{
    public class ProductListItemViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl  { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public int NutritionTypes { get; set; }
    }
}
