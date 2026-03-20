namespace FreshNFluffy.Services
{
    using FreshNFluffy.Data;
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Services.Interfaces;
    using FreshNFluffy.ViewModels.Reviews;
    using Microsoft.EntityFrameworkCore;

    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext dbContext;

        public ReviewService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddReviewAsync(CreateReviewViewModel model, string userId)
        {
            Review review = new Review
            {
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment,
                UserId = userId,
                CreatedOn = DateTime.UtcNow
            };

            await this.dbContext.Reviews.AddAsync(review);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReviewListItemViewModel>> GetByProductIdAsync(int productId)
        {
            return await dbContext.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedOn)
                .Select(r => new ReviewListItemViewModel
                {
                    Rating = r.Rating,
                    Comment = r.Comment,
                    UserName = r.User.UserName!,
                    CreatedOn = r.CreatedOn
                })
                .ToListAsync();
        }
    }
}
