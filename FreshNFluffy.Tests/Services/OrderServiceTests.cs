using FreshNFluffy.Data;
using FreshNFluffy.Data.Models;
using FreshNFluffy.Data.Models.Enum;
using FreshNFluffy.Data.Repository;
using FreshNFluffy.Services;
using FreshNFluffy.ViewModels.Orders;
using Microsoft.EntityFrameworkCore;

namespace FreshNFluffy.Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrderSuccessfully()
        {
            //Arrange 
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            OrderCreateViewModel model = new OrderCreateViewModel
            {
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
            };

            string userId = "user-1";

            //Act
            int orderId = await orderService.CreateOrderAsync(model, userId);


            //Assert
            Assert.True(orderId > 0);

            OrderRequest? order = await dbContext.OrderRequests.FirstOrDefaultAsync();

            Assert.NotNull(order);
            Assert.Equal("Test User", order!.CustomerName);
            Assert.Equal(userId, order.UserId);
        }

        [Fact]
        public async Task AddItemAsync_ShouldAddNewItem_WhenDataIsValid()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Product product = new Product
            {
                ProductId = 1,
                Name = "Cake",
                Price = 10
            };

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            await dbContext.Products.AddAsync(product);
            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            AddOrderItemInputModel model = new AddOrderItemInputModel
            {
                OrderRequestId = 1,
                ProductId = 1,
                Quantity = 2
            };

            //Act
            bool result = await orderService.AddItemAsync(model);

            //Assert
            Assert.True(result);

            OrderItem? item = await dbContext.OrderItems.FirstOrDefaultAsync();

            Assert.NotNull(item);
            Assert.Equal(2, item!.Quantity);
            Assert.Equal(10, item.UnitPrice);
        }

        [Fact]
        public async Task AddItemAsync_ShouldIncreaseQuantity_WhenItemExists()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Product product = new Product
            {
                ProductId = 1,
                Name = "Cake",
                Price = 10
            };

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            OrderItem existingItem = new OrderItem
            {
                OrderItemId = 1,
                OrderRequestId = 1,
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 10
            };

            await dbContext.Products.AddAsync(product);
            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.OrderItems.AddAsync(existingItem);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            AddOrderItemInputModel model = new AddOrderItemInputModel
            {
                OrderRequestId = 1,
                ProductId = 1,
                Quantity = 3
            };

            //Act
            bool result = await orderService.AddItemAsync(model);

            //Assert
            Assert.True(result);

            List<OrderItem> orderItems = await dbContext.OrderItems.ToListAsync();

            Assert.Single(orderItems);
            Assert.Equal(5, orderItems[0].Quantity);
        }

        [Fact]
        public async Task AddItemAsync_ShouldFail_WhenQuantityIsInvalid()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Product product = new Product
            {
                ProductId = 1,
                Name = "Cake",
                Price = 10
            };

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            await dbContext.Products.AddAsync(product);
            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            AddOrderItemInputModel model = new AddOrderItemInputModel
            {
                OrderRequestId = 1,
                ProductId = 1,
                Quantity = 0
            };

            //Act
            bool result = await orderService.AddItemAsync(model);

            //Assert
            Assert.False(result);
            Assert.Empty(dbContext.OrderItems);
        }

        [Fact]
        public async Task AddItemAsync_ShouldFail_WhenOrderIsLocked()
        {
            //Arrange 
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Product product = new Product
            {
                ProductId = 1,
                Name = "Cake",
                Price = 10
            };

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Cancelled,
                UserId = "user-1"
            };

            await dbContext.Products.AddAsync(product);
            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            AddOrderItemInputModel model = new AddOrderItemInputModel
            {
                OrderRequestId = 1,
                ProductId = 1,
                Quantity = 2
            };

            //Act
            bool result = await orderService.AddItemAsync(model);

            //Assert
            Assert.False(result);
            Assert.Empty(dbContext.OrderItems);
        }
        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenTransitionIsValid()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UpdateStatusAsync(1, OrderStatus.Confirmed);

            //Assert
            Assert.True(result);

            OrderRequest? updatedOrder = await dbContext.OrderRequests.FirstOrDefaultAsync(o => o.OrderRequestId == 1);

            Assert.NotNull(updatedOrder);
            Assert.Equal(OrderStatus.Confirmed, updatedOrder!.Status);
        }
        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnFalse_WhenTransitionIsInvalid()
        {
            //Arrange

            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UpdateStatusAsync(1, OrderStatus.Completed);

            //Assert
            Assert.False(result);

            OrderRequest? unchangedOrder = await dbContext.OrderRequests.FirstOrDefaultAsync(o => o.OrderRequestId == 1);

            Assert.NotNull(unchangedOrder);
            Assert.Equal(OrderStatus.Pending, unchangedOrder!.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnFalse_WhenOrderIsCompleted()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Completed,
                UserId = "user-1"
            };

            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UpdateStatusAsync(1, OrderStatus.Confirmed);

            //Assert
            Assert.False(result);

            OrderRequest unchangedOrder = await dbContext.OrderRequests.FirstAsync();
            Assert.Equal(OrderStatus.Completed, unchangedOrder.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnFalse_WhenOrderIsCancelled()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Cancelled,
                UserId = "user-1"
            };

            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UpdateStatusAsync(1, OrderStatus.Confirmed);

            //Assert
            Assert.False(result);

            OrderRequest unchangedOrder = await dbContext.OrderRequests.FirstAsync();
            Assert.Equal(OrderStatus.Cancelled, unchangedOrder.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UpdateStatusAsync(999, OrderStatus.Confirmed);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UserCanAccessOrderAsync_ShouldReturnTrue_WhenUserOwnsOrder()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Owner User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UserCanAccessOrderAsync(1, "user-1", false);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserCanAccessOrderAsync_ShouldReturnFalse_WhenUserDoesNotOwnOrder()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Owner User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "owner-user"
            };

            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UserCanAccessOrderAsync(1, "another-user", false);

            //Assert
            Assert.False(result);
        }
        [Fact]
        public async Task UserCanAccessOrderAsync_ShouldReturnTrue_WhenUserIsAdmin()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Owner User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "owner-user"
            };

            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UserCanAccessOrderAsync(1, "admin-user", true);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveItemAsync_ShouldRemoveItemSuccessfully()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Product product = new Product
            {
                ProductId = 1,
                Name = "Cake",
                Price = 10
            };

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            OrderItem orderItem = new OrderItem
            {
                OrderItemId = 1,
                OrderRequestId = 1,
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 10
            };

            await dbContext.Products.AddAsync(product);
            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.OrderItems.AddAsync(orderItem);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.RemoveItemAsync(1);

            //Assert
            Assert.True(result);
            Assert.Empty(dbContext.OrderItems);
        }

        [Fact]
        public async Task GetSummaryAsync_ShouldReturnCorrectTotal()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Product firstProduct = new Product
            {
                ProductId = 1,
                Name = "Cake",
                Price = 10
            };

            Product secondProduct = new Product
            {
                ProductId = 2,
                Name = "Cookie",
                Price = 5
            };

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Notes = "Test note",
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            OrderItem firstItem = new OrderItem
            {
                OrderItemId = 1,
                OrderRequestId = 1,
                ProductId = 1,
                Product = firstProduct,
                Quantity = 2,
                UnitPrice = 10
            };

            OrderItem secondItem = new OrderItem
            {
                OrderItemId = 2,
                OrderRequestId = 1,
                ProductId = 2,
                Product = secondProduct,
                Quantity = 3,
                UnitPrice = 5
            };

            await dbContext.Products.AddRangeAsync(firstProduct, secondProduct);
            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.OrderItems.AddRangeAsync(firstItem, secondItem);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            OrderSummaryViewModel? result = await orderService.GetSummaryAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.OrderRequestId);
            Assert.Equal("Test User", result.CustomerName);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(35, result.TotalPrice);
        }

        [Fact]
        public async Task UpdateItemQuantity_ShouldReturnFalse_WhenQunatityIsInvalid()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Product product = new Product
            {
                ProductId = 1,
                Name = "Cake",
                Price = 10
            };

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            OrderItem orderItem = new OrderItem
            {
                OrderItemId = 1,
                OrderRequestId = 1,
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 10
            };

            await dbContext.Products.AddAsync(product);
            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.OrderItems.AddAsync(orderItem);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UpdateItemQuantityAsync(1, 0);

            //Assert
            Assert.False(result);

            OrderItem? unchangedItem = await dbContext.OrderItems.FirstOrDefaultAsync();
            Assert.NotNull(unchangedItem);  
            Assert.Equal(2, unchangedItem!.Quantity);
        }

        [Fact]
        public async Task UpdateItemQuantityAsync_ShouldUpdateQuantitySuccessfully()
        {
            //Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options);

            Product product = new Product
            {
                ProductId = 1,
                Name = "Cake",
                Price = 10
            };

            OrderRequest orderRequest = new OrderRequest
            {
                OrderRequestId = 1,
                CustomerName = "Test User",
                PhoneNumber = "123456789",
                PickupDate = DateTime.UtcNow.AddDays(1),
                Status = OrderStatus.Pending,
                UserId = "user-1"
            };

            OrderItem orderItem = new OrderItem
            {
                OrderItemId = 1,
                OrderRequestId = 1,
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 10
            };


            await dbContext.Products.AddAsync(product); 
            await dbContext.OrderRequests.AddAsync(orderRequest);
            await dbContext.OrderItems.AddAsync(orderItem);
            await dbContext.SaveChangesAsync();

            OrderRepository orderRepository = new OrderRepository(dbContext);
            OrderService orderService = new OrderService(orderRepository);

            //Act
            bool result = await orderService.UpdateItemQuantityAsync(1, 5); 

            Assert.True(result);

            OrderItem updatedItem = await dbContext.OrderItems.FirstAsync();
            Assert.Equal(5, updatedItem.Quantity);
        }
    }
}
