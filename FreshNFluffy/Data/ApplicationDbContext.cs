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
        public virtual DbSet<Review> Reviews { get; set; } = null!;

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

            builder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Chocolate Fudge Cake", Description = "Rich chocolate sponge with fudge frosting.", Price = 29.90m, ImageUrl = "https://example.com/img/chocolate-fudge-cake.jpg", CategoryId = 1, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 2, Name = "Carrot Walnut Cake", Description = "Moist carrot cake with walnuts and cinnamon.", Price = 27.50m, ImageUrl = "https://example.com/img/carrot-walnut-cake.jpg", CategoryId = 1, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 3, Name = "Vegan Chocolate Cake", Description = "Plant-based chocolate cake, dairy-free.", Price = 30.90m, ImageUrl = "https://example.com/img/vegan-chocolate-cake.jpg", CategoryId = 1, NutritionTypes = NutritionTypes.Vegan | NutritionTypes.LactoseFree },

                new Product { ProductId = 4, Name = "Butter Croissant", Description = "Classic French croissant, flaky and buttery.", Price = 3.20m, ImageUrl = "https://example.com/img/butter-croissant.jpg", CategoryId = 2, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 5, Name = "Pain au Chocolat", Description = "Croissant pastry filled with dark chocolate.", Price = 3.80m, ImageUrl = "https://example.com/img/pain-au-chocolat.jpg", CategoryId = 2, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 6, Name = "Vegan Apple Turnover", Description = "Crispy puff pastry with spiced apples, vegan.", Price = 4.10m, ImageUrl = "https://example.com/img/vegan-apple-turnover.jpg", CategoryId = 2, NutritionTypes = NutritionTypes.Vegan | NutritionTypes.LactoseFree },

                new Product { ProductId = 7, Name = "Sourdough Loaf", Description = "Naturally leavened sourdough bread.", Price = 6.50m, ImageUrl = "https://example.com/img/sourdough-loaf.jpg", CategoryId = 3, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 8, Name = "Wholegrain Bread", Description = "Hearty wholegrain loaf, great for sandwiches.", Price = 5.90m, ImageUrl = "https://example.com/img/wholegrain-bread.jpg", CategoryId = 3, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 9, Name = "Gluten-Free Seed Bread", Description = "Gluten-free loaf with mixed seeds.", Price = 7.40m, ImageUrl = "https://example.com/img/gluten-free-seed-bread.jpg", CategoryId = 3, NutritionTypes = NutritionTypes.GlutenFree | NutritionTypes.Vegan | NutritionTypes.LactoseFree },

                new Product { ProductId = 10, Name = "Chocolate Chip Cookies", Description = "Soft cookies with dark chocolate chips.", Price = 6.90m, ImageUrl = "https://example.com/img/choc-chip-cookies.jpg", CategoryId = 4, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 11, Name = "Oat & Raisin Cookies", Description = "Oat cookies with raisins and a hint of cinnamon.", Price = 6.20m, ImageUrl = "https://example.com/img/oat-raisin-cookies.jpg", CategoryId = 4, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 12, Name = "Sugar-Free Almond Cookies", Description = "Crunchy almond cookies without added sugar.", Price = 7.10m, ImageUrl = "https://example.com/img/sugar-free-almond-cookies.jpg", CategoryId = 4, NutritionTypes = NutritionTypes.SugarFree | NutritionTypes.Keto | NutritionTypes.GlutenFree },

                new Product { ProductId = 13, Name = "Classic Cheesecake", Description = "Creamy cheesecake with vanilla and biscuit base.", Price = 22.90m, ImageUrl = "https://example.com/img/classic-cheesecake.jpg", CategoryId = 5, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 14, Name = "Keto Cheesecake Cup", Description = "Low-carb cheesecake dessert cup.", Price = 8.90m, ImageUrl = "https://example.com/img/keto-cheesecake-cup.jpg", CategoryId = 5, NutritionTypes = NutritionTypes.Keto | NutritionTypes.SugarFree | NutritionTypes.GlutenFree },
                new Product { ProductId = 15, Name = "Vegan Chocolate Mousse", Description = "Silky vegan mousse, dairy-free.", Price = 9.20m, ImageUrl = "https://example.com/img/vegan-chocolate-mousse.jpg", CategoryId = 5, NutritionTypes = NutritionTypes.Vegan | NutritionTypes.LactoseFree },

                new Product { ProductId = 16, Name = "Vanilla Sweet Scone", Description = "Soft vanilla scone, perfect with tea.", Price = 2.80m, ImageUrl = "https://example.com/img/vanilla-scone.jpg", CategoryId = 6, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 17, Name = "Blueberry Sweet Scone", Description = "Buttery scone with blueberries.", Price = 3.10m, ImageUrl = "https://example.com/img/blueberry-scone.jpg", CategoryId = 6, NutritionTypes = NutritionTypes.None },

                new Product { ProductId = 18, Name = "Cheddar Salty Scone", Description = "Savory scone with cheddar cheese.", Price = 3.40m, ImageUrl = "https://example.com/img/cheddar-scone.jpg", CategoryId = 7, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 19, Name = "Olive & Herb Salty Scone", Description = "Savory scone with olives and herbs.", Price = 3.60m, ImageUrl = "https://example.com/img/olive-herb-scone.jpg", CategoryId = 7, NutritionTypes = NutritionTypes.None },
                new Product { ProductId = 20, Name = "Keto Salty Scone", Description = "Low-carb savory scone, keto friendly.", Price = 4.20m, ImageUrl = "https://example.com/img/keto-salty-scone.jpg", CategoryId = 7, NutritionTypes = NutritionTypes.Keto | NutritionTypes.GlutenFree }
            );
        }
    }
}
