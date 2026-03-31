using FreshNFluffy.Data;
using FreshNFluffy.Data.Models;
using FreshNFluffy.Data.Models.Enum;
using FreshNFluffy.Data.Repository;
using FreshNFluffy.Services;
using FreshNFluffy.ViewModels.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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
    }
}
