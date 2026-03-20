
namespace FreshNFluffy.ViewModels.Reviews
{
    using System.ComponentModel.DataAnnotations;
    using static FreshNFluffy.Common.EntityValidation.Review;
    public class CreateReviewViewModel
    {
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(CommentMaxLength, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; } = null!;

    }
}
