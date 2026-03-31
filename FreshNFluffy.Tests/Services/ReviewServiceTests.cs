using FreshNFluffy.Data;
using FreshNFluffy.Data.Models;
using FreshNFluffy.Services;
using FreshNFluffy.ViewModels.Reviews;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace FreshNFluffy.Tests.Services
{
    public class ReviewServiceTests
    {
        [Fact]
        public async Task AddReviewAsync_ShouldAddReviewSuccessfully()
        {
            //Arrange

            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            ReviewService reviewService = new ReviewService(dbContext);

            CreateReviewViewModel model = new CreateReviewViewModel
            {
                ProductId = 1,
                Rating = 5,
                Comment = "Great product!"
            };


            string userId = "user-1";

            //Act
            await reviewService.AddReviewAsync(model, userId);

            //Assert
            Review? review = await dbContext.Reviews.FirstOrDefaultAsync();

            Assert.NotNull(review);
            Assert.Equal(1, review!.ProductId);
            Assert.Equal(5, review.Rating);
            Assert.Equal("Great product!", review.Comment);
            Assert.Equal("user-1", review.UserId);
        }

        [Fact]
        public async Task GetByProductIdAsync_ShouldReturnReviewsOrderedByCreatedOnDescending()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            IdentityUser firstUser = new IdentityUser
            {
                Id = "user-1",
                UserName = "firstUser"
            };

            IdentityUser secondUser = new IdentityUser
            {
                Id = "user-2",
                UserName = "secondUser"
            };

            await dbContext.Users.AddRangeAsync(firstUser, secondUser);

            await dbContext.Reviews.AddRangeAsync(
                new Review
            {
                ReviewId = 1,
                ProductId = 1,
                UserId = "user-1",
                User = firstUser,
                Rating = 4,
                Comment = "Older review",
                CreatedOn = new DateTime(2026, 3, 1)
            },
                new Review
                {
                    ReviewId = 2,
                    ProductId = 1,
                    UserId = "user-2",
                    User = secondUser,
                    Rating = 5,
                    Comment = "Newest review",
                    CreatedOn = new DateTime(2026, 3, 2)
                },
                new Review
                {
                    ReviewId = 3,
                    ProductId = 2,
                    UserId = "user-1",
                    User = firstUser,
                    Rating = 3,
                    Comment = "Other product review",
                    CreatedOn = new DateTime(2026, 3, 3)
                });

            await dbContext.SaveChangesAsync();

            ReviewService reviewService = new ReviewService(dbContext);

            //Act
            IEnumerable<ReviewListItemViewModel> result = await reviewService.GetByProductIdAsync(1);

            List<ReviewListItemViewModel> reviews = result.ToList();

            //Assert
            Assert.Equal(2, reviews.Count());

            Assert.Equal("Newest review", reviews[0].Comment);
            Assert.Equal("secondUser", reviews[0].UserName);

            Assert.Equal("Older review", reviews[1].Comment);
            Assert.Equal("firstUser", reviews[1].UserName);
        }
    }
}
