namespace FreshNFluffy.Data
{
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Models.Enum;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;


    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> DbContextOptions)
            : base(DbContextOptions)
        {
        }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<OrderRequest> OrderRequests { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.OrderRequest)
                .WithMany(or => or.Items)
                .HasForeignKey(oi => oi.OrderRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>().HasData(
            new Category
            {
                CategoryId = 1,
                Name = "Cakes",
                NutritionTypes = NutritionTypes.None
            },
            new Category
            {
                CategoryId = 2,
                Name = "Pastries",
                NutritionTypes = NutritionTypes.LactoseFree
            },
            new Category
            {
                CategoryId = 3,
                Name = "Bread",
                NutritionTypes = NutritionTypes.GlutenFree
            },
            new Category
            {
                CategoryId = 4,
                Name = "Cookies",
                NutritionTypes = NutritionTypes.SugarFree
            },
            new Category
            {
                CategoryId = 5,
                Name = "Desserts",
                NutritionTypes = NutritionTypes.SugarFree | NutritionTypes.GlutenFree
            },
            new Category
            {
                CategoryId = 6,
                Name = "Sweet Scones",
                NutritionTypes = NutritionTypes.LactoseFree | NutritionTypes.Vegan
            },
            new Category
            {
                CategoryId = 7,
                Name = "Salty Scones",
                NutritionTypes = NutritionTypes.Keto | NutritionTypes.GlutenFree
            });
        }
    }
}
