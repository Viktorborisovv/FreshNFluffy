using FreshNFluffy.ViewModels.Reviews;

namespace FreshNFluffy.ViewModels.Products
{
    public class ProductDetailsViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public string CategoryName { get; set; } = null!;
        public string NutritionText { get; set; } = "None";

        public IEnumerable<ReviewListItemViewModel> Reviews { get; set; }
            = new List<ReviewListItemViewModel>();

        public CreateReviewViewModel NewReview { get; set; } 
            = new CreateReviewViewModel();
    }
}
