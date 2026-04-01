namespace FreshNFluffy.Tests.Services
{
    using FreshNFluffy.Services;

    using FreshNFluffy.ViewModels.Products;

    using FreshNFluffy.Data;
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Repository;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using FreshNFluffy.Data.Models.Enum;

    public class ProductServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedProductsAndCategories()
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
        public async Task GetDetailsAsync_ShouldReturnProduct_WhenExists()
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

            IdentityUser user = new IdentityUser
            {
                Id = "user-1",
                UserName = "testuser",
            };

            Product product = new Product
            {
                ProductId = 1,
                Name = "Chocolate Cake",
                Description = "Rich chocolate cake",
                Price = 12.50m,
                CategoryId = 1,
                Category = category, 
                NutritionTypes = NutritionTypes.GlutenFree
            };

            Review review = new Review
            {
                ReviewId = 1,
                ProductId = 1,
                Product = product,
                UserId = "user-1",
                User = user,
                Rating = 5,
                Comment = "Amazing!",
                CreatedOn = new DateTime(2026, 3, 31)
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.Users.AddAsync(user);
            await dbContext.Products.AddAsync(product);
            await dbContext.Reviews.AddAsync(review);
            await dbContext.SaveChangesAsync();

            ProductRepository productRepository = new ProductRepository(dbContext);
            ProductService productService = new ProductService(productRepository);

            //Act
            ProductDetailsViewModel? result = await productService.GetDetailsAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.ProductId);
            Assert.Equal("Chocolate Cake", result.Name);
            Assert.Equal("Rich chocolate cake", result.Description);
            Assert.Equal(12.50m, result.Price);
            Assert.Equal("Cakes", result.CategoryName);
            Assert.Equal("GlutenFree", result.NutritionText);

            Assert.Single(result.Reviews);
            Assert.Equal("Amazing!", result.Reviews.First().Comment);

            Assert.NotNull(result.NewReview);
            Assert.Equal(1, result.NewReview.ProductId);
        }

        [Fact]
        public async Task GetDetailsAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            ProductRepository productRepository = new ProductRepository(dbContext);
            ProductService productService = new ProductService(productRepository);
            
            //Act
            ProductDetailsViewModel? result = await productService.GetDetailsAsync(999);
            
            //Assert
            Assert.Null(result);
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

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenCategoryDoesNotExist()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            ProductRepository productRepository = new ProductRepository(dbContext);
            ProductService productService = new ProductService(productRepository);

            ProductFormViewModel model = new ProductFormViewModel
            {
                Name = "Invalid Cake",
                Description = "Invalid Description",
                Price = 10,
                CategoryId = 999,
                NutritionTypes = 0
            };

            //Act
            int result = await productService.CreateAsync(model);

            //Assert
            Assert.Equal(0, result);
            Assert.Empty(dbContext.Products);
        }

        [Fact]
        public async Task EditAsync_ShouldUpdateProduct_WhenCategoryIsValid()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Category oldCategory = new Category
            {
                CategoryId = 1,
                Name = "Cakes",
            };

            Category newCategory = new Category
            {
                CategoryId = 2,
                Name = "Cookies",
            };

            Product product = new Product
            {
                ProductId = 1,
                Name = "Old Cake",
                Description = "Old Description",
                Price = 10,
                CategoryId = 1,
                NutritionTypes = 0
            };

            await dbContext.Categories.AddRangeAsync(oldCategory, newCategory);
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();

            ProductRepository productRepository = new ProductRepository(dbContext);
            ProductService productService = new ProductService(productRepository);

            ProductFormViewModel model = new ProductFormViewModel
            {
                ProductId = 1,
                Name = "Updated Cake",
                Description = "Updated Description",
                Price = 15,
                ImageUrl = "test.jpg",
                CategoryId = 2,
                NutritionTypes = 1
            };

            //Act
            bool result = await productService.EditAsync(model);

            //Assert
            Assert.True(result);

            Product? editedProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

            Assert.NotNull(editedProduct);
            Assert.Equal("Updated Cake", editedProduct!.Name);
            Assert.Equal("Updated Description", editedProduct.Description);
            Assert.Equal(15, editedProduct.Price);
            Assert.Equal("test.jpg", editedProduct.ImageUrl);
            Assert.Equal(2, editedProduct.CategoryId);
            Assert.Equal(1, (int)editedProduct.NutritionTypes);
        }

        [Fact]
        public async Task EditAsync_ShouldReturnFalse_WhenProductIdIsNull()
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
                ProductId = null,
                Name = "Updated Cake",
                Description = "Updated Description",
                Price = 15,
                CategoryId = 1,
                NutritionTypes = 1
            };

            //Act
            bool result = await productService.EditAsync(model);

            //Assert
            Assert.False(result);
            Assert.Empty(dbContext.Products);
        }

        [Fact]
        public async Task EditAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Category category = new Category
            {
                CategoryId = 1,
                Name = "Cakes",
            };

            Product product = new Product
            {
                ProductId = 1,
                Name = "Old Cake",
                Description = "Old Description",
                Price = 10,
                CategoryId = 1,
                NutritionTypes = 0
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();

            ProductRepository productRepository = new ProductRepository(dbContext);
            ProductService productService = new ProductService(productRepository);

            ProductFormViewModel model = new ProductFormViewModel
            {
                ProductId = 1,
                Name = "Updated Cake",
                Description = "Updated Description",
                Price = 15,
                ImageUrl = "test.jpg",
                CategoryId = 888,
                NutritionTypes = 1
            };

            //Act
            bool result = await productService.EditAsync(model);

            //Assert
            Assert.False(result);

            Product? unchangedProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

            Assert.NotNull(unchangedProduct);
            Assert.Equal("Old Cake", unchangedProduct!.Name);
            Assert.Equal("Old Description", unchangedProduct.Description);
            Assert.Equal(10, unchangedProduct.Price);
            Assert.Equal(1, unchangedProduct.CategoryId);
        }
    }
}