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
    }
}
