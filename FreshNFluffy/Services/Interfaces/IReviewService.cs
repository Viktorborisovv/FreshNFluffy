namespace FreshNFluffy.Services.Interfaces
{
    using FreshNFluffy.ViewModels.Reviews;

    public interface IReviewService
    {
        Task AddReviewAsync(CreateReviewViewModel model, string userId);
        Task<IEnumerable<ReviewListItemViewModel>> GetByProductIdAsync(int productId);
    }
}
