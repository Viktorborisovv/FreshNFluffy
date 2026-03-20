namespace FreshNFluffy.Controllers
{
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Services.Interfaces;
    using FreshNFluffy.ViewModels.Reviews;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class ReviewsController : Controller
    {
        private readonly IReviewService reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "NewReview")]CreateReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide a valid rating and comment.";
                return RedirectToAction("Details", "Products", new { id = model.ProductId });
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await reviewService.AddReviewAsync(model, userId);

            TempData["Success"] = "Review added successfully.";

            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }
    }
}
