namespace FreshNFluffy.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidation.Category;

    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } =
               new HashSet<Product>();
    }
}
