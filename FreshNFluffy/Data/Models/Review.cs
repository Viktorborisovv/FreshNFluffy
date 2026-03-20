namespace FreshNFluffy.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static Common.EntityValidation.Review;

    public class Review
    {
        [Key]
        [Required]
        public int ReviewId { get; set; }

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public virtual IdentityUser User { get; set; } = null!;

        [Range(RatingMin, RatingMax)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(CommentMaxLength)]
        public string Comment { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
