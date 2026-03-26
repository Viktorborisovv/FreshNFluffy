namespace FreshNFluffy.Tests.Services
{
    using FreshNFluffy.Services;

    using FreshNFluffy.ViewModels.Products;

    using FreshNFluffy.Data;
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Repository;

    using Microsoft.EntityFrameworkCore;
    public class ProductServiceTests
    {
        [Fact]
        public async Task GetTaskAsync_ShouldReturnPagedProductsAndCategories()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Category category = new Category
            {
                CategoryId = 1,
                Name = "Cakes",
            };

            await dbContext.Categories.AddAsync(category);

            await dbContext.Products.AddRangeAsync(
                new Product
                {
                    ProductId = 1,
                    Name = "Chocolate Cake",
                    Price = 10,
                    CategoryId = 1,
                    Category = category,
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Vanilla Cake",
                    Price = 12,
                    CategoryId = 1,
                    Category = category,
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Red Velvet Cake",
                    Price = 15,
                    CategoryId = 1,
                    Category = category,
                });

            await dbContext.SaveChangesAsync();

            ProductRepository productRepository = new ProductRepository(dbContext);
            ProductService productService = new ProductService(productRepository);


            ProductQueryViewModel query = new ProductQueryViewModel
            {
                CurrentPage = 1,
                ProductsPerPage = 2,
            };

            //Act
            ProductQueryViewModel result = await productService.GetAllAsync(query);

            //Assert
            Assert.NotNull(result);
            Assert.Single(result.Categories);
            Assert.Equal(3, result.TotalProducts);
            Assert.Equal(2, result.Products.Count());
            Assert.Equal("Chocolate Cake", result.Products.First().Name);
            Assert.Equal("Red Velvet Cake", result.Products.Last().Name);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateProductAndReturnId_WhenCategoryExists()
        {
            //Arrange 
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Category category = new Category
            {
                CategoryId = 1,
                Name = "Cakes",
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();


            ProductRepository productRepository = new ProductRepository(dbContext);
            ProductService productService = new ProductService(productRepository);

            ProductFormViewModel model = new ProductFormViewModel
            {
                Name = "Test Cake",
                Description = "Test Description",
                Price = 10,
                ImageUrl = null,
                CategoryId = 1,
                NutritionTypes = 0
            };

            //Act
            int resultId = await productService.CreateAsync(model);

            //Assert
            Assert.True(resultId > 0);

            Product? createdProduct = await dbContext.Products.FirstOrDefaultAsync();

            Assert.NotNull(createdProduct);
            Assert.Equal("Test Cake", createdProduct!.Name);
        }
    }
}