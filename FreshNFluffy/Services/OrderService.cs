namespace FreshNFluffy.Services
{
    using FreshNFluffy.Data;
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Models.Enum;

    using FreshNFluffy.Services.Interfaces;

    using FreshNFluffy.ViewModels.Orders;
    using Microsoft.EntityFrameworkCore;

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<int> CreateOrderAsync(OrderCreateViewModel model)
        {
            OrderRequest order = new OrderRequest
            {
                CustomerName = model.CustomerName,
                PhoneNumber = model.PhoneNumber,
                PickupDate = model.PickupDate,
                Notes = model.Notes,
                CreatedOn = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            dbContext.OrderRequests.Add(order);

            await dbContext.SaveChangesAsync();
            return order.OrderRequestId;
        }
        public async Task<AddOrderItemViewModel?> GetAddItemsFormAsync(int orderRequestId)
        {
            bool orderExists = await dbContext.OrderRequests
                .AsNoTracking()
                .AnyAsync(oe => oe.OrderRequestId == orderRequestId);

            if (!orderExists)
                return null;

            List<ProductSelectViewModel> products = await dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new ProductSelectViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToListAsync();

            List<OrderItemRowViewModel> items = await dbContext.OrderItems
                .AsNoTracking()
                .Where(i => i.OrderRequestId == orderRequestId)
                .Include(i => i.Product)
                .OrderBy(i => i.OrderItemId)
                .Select(i => new OrderItemRowViewModel
                {
                    OrderItemId = i.OrderItemId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                })
                .ToListAsync();

            return new AddOrderItemViewModel
            {
                OrderRequestId = orderRequestId,
                Products = products,
                CurrentItems = items,
                NewItem = new AddOrderItemInputModel
                {
                    OrderRequestId = orderRequestId,
                    Quantity = 1
                }
            };
        }

        public async Task<bool> AddItemAsync(AddOrderItemInputModel model)
        {
            if (model.Quantity <= 0)
                return false;

            bool orderExists = await dbContext.OrderRequests
                .AsNoTracking()
                .AnyAsync(o => o.OrderRequestId == model.OrderRequestId);

            if (!orderExists)
                return false;

            Product? product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

            if (product == null)
                return false;

            OrderItem? existingItem = await dbContext.OrderItems
                .FirstOrDefaultAsync(ei => ei.OrderRequestId == model.OrderRequestId
                                        && ei.ProductId == model.ProductId);


            if (existingItem != null)
            {
                existingItem.Quantity += model.Quantity;

                await dbContext.SaveChangesAsync();
                return true;
            }

            OrderItem newItem = new OrderItem
            {
                OrderRequestId = model.OrderRequestId,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                UnitPrice = product.Price
            };

            dbContext.OrderItems.Add(newItem);

            await dbContext.SaveChangesAsync();
            return true;
        }



        public async Task<OrderSummaryViewModel?> GetSummaryAsync(int orderRequestId)
        {
            var order = await dbContext.OrderRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderRequestId == orderRequestId);

            if (order == null)
                return null;

            var items = await dbContext.OrderItems
                .AsNoTracking()
                .Where(i => i.OrderRequestId == orderRequestId)
                .Include(i => i.Product)
                .OrderBy(i => i.OrderItemId)
                .Select(i => new OrderItemRowViewModel
                {
                    OrderItemId = i.OrderItemId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                })
                .ToListAsync();

            decimal total = items.Sum(i => i.RowTotal);

            return new OrderSummaryViewModel
            {
                OrderRequestId = order.OrderRequestId,
                CustomerName = order.CustomerName,
                PhoneNumber = order.PhoneNumber,
                PickupDate = order.PickupDate,
                Notes = order.Notes,
                Items = items,
                TotalPrice = total
            };
        }
    }
}
