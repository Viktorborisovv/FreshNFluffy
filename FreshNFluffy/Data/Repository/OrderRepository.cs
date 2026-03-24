namespace FreshNFluffy.Data.Repository
{
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Models.Enum;

    using FreshNFluffy.Data.Repository.Contracts;

    using FreshNFluffy.ViewModels.Orders;
    using FreshNFluffy.ViewModels.Orders.Management;

    using Microsoft.EntityFrameworkCore;
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddOrderRequestAsync(OrderRequest orderRequest)
        {
            await dbContext.OrderRequests.AddAsync(orderRequest);
        }
        public async Task<OrderRequest?> GetOrderRequestByIdAsync(int orderRequestId)
        {
            return await dbContext.OrderRequests
                .FirstOrDefaultAsync(o => o.OrderRequestId == orderRequestId);
        }
        public async Task<OrderRequest?> GetOrderRequestByIdAsNoTrackingAsync(int orderRequestId)
        {
            return await dbContext.OrderRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderRequestId == orderRequestId);
        }
        public async Task<bool> OrderRequestExistsAsync(int orderRequestId)
        {
            return await dbContext.OrderRequests
                .AnyAsync(o => o.OrderRequestId == orderRequestId);
        }
        public async Task<bool> IsOrderLockedAsync(int orderRequestId)
        {
            return await dbContext.OrderRequests
                .AsNoTracking()
                .AnyAsync(o => o.OrderRequestId == orderRequestId &&
                (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled));
        }
        public async Task<IEnumerable<ProductSelectViewModel>> GetProductsForOrderAsync()
        {
            return await dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new ProductSelectViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<OrderItemRowViewModel>> GetOrderItemsByOrderRequestIdAsync(int orderRequestId)
        {
            return await dbContext.OrderItems
                .AsNoTracking()
                .Where(oi => oi.OrderRequestId == orderRequestId)
                .Include(oi => oi.Product)
                .OrderBy(oi => oi.OrderItemId)
                .Select(oi => new OrderItemRowViewModel
                {
                    OrderItemId = oi.OrderItemId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                })
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }
        public async Task<OrderItem?> GetOrderItemByOrderAndProductAsync(int orderRequestId, int productId)
        {
            return await dbContext.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderRequestId == orderRequestId &&
                                           oi.ProductId == productId);
        }
        public async Task AddOrderItemAsync(OrderItem orderItem)
        {
            await dbContext.OrderItems.AddAsync(orderItem);
        }

        public async Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId)
        {
            return await dbContext.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);
        }
        public void RemoveOrderItem(OrderItem orderItem)
        {
            dbContext.OrderItems.Remove(orderItem);
        }

        public async Task<IEnumerable<OrderListItemViewModel>> GetOrdersForManagementAsync(int? statusFilter, string? searchTerm)
        {
            IQueryable<OrderRequest> orderRequestsQuery = dbContext.OrderRequests
                .AsNoTracking();

            if (statusFilter.HasValue)
            {
                orderRequestsQuery = orderRequestsQuery
                    .Where(or => (int)or.Status == statusFilter.Value);
            }
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();

                bool isNumber = int.TryParse(term, out int parsedOrderId);

                orderRequestsQuery = orderRequestsQuery
                    .Where(or => or.CustomerName.Contains(term) ||
                           or.PhoneNumber.Contains(term) ||
                           (isNumber && or.OrderRequestId == parsedOrderId));
            }

            return await orderRequestsQuery
                .GroupJoin(
                dbContext.OrderItems.AsNoTracking(),
                orderRequest => orderRequest.OrderRequestId,
                orderItem => orderItem.OrderRequestId,
                (orderRequest, orderItems) => new OrderListItemViewModel
                {
                    OrderRequestId = orderRequest.OrderRequestId,
                    CustomerName = orderRequest.CustomerName,
                    CreatedOn = orderRequest.CreatedOn,
                    PickupDate = orderRequest.PickupDate,
                    Status = orderRequest.Status.ToString(),
                    StatusValue = (int)orderRequest.Status,
                    TotalPrice = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
                })
                .OrderByDescending(o => o.CreatedOn)
                .ThenByDescending(o => o.OrderRequestId)
                .ToListAsync();
        }

        public async Task<bool> UserOwnsOrderAsync(int orderRequestId, string userId)
        {
            return await dbContext.OrderRequests
                .AsNoTracking()
                .AnyAsync(or => or.OrderRequestId == orderRequestId && or.UserId == userId);
        }
        public Task<int> SaveChangesAsync()
        {
            return dbContext.SaveChangesAsync();
        }
    }
}
