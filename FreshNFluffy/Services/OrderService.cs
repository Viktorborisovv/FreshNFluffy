namespace FreshNFluffy.Services
{
    using FreshNFluffy.Data;
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Models.Enum;

    using FreshNFluffy.Services.Interfaces;
    using FreshNFluffy.ViewModels.Orders;
    using FreshNFluffy.ViewModels.Orders.Management;

    using Microsoft.EntityFrameworkCore;

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> CreateOrderAsync(OrderCreateViewModel model, string userId)
        {
            OrderRequest order = new OrderRequest
            {
                CustomerName = model.CustomerName,
                PhoneNumber = model.PhoneNumber,
                PickupDate = model.PickupDate,
                Notes = model.Notes,
                CreatedOn = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                UserId = userId
            };

            dbContext.OrderRequests.Add(order);

            await dbContext.SaveChangesAsync();

            return order.OrderRequestId;
        }
        public async Task<AddOrderItemViewModel?> GetAddItemsFormAsync(int orderRequestId)
        {
            var orderRequest= await dbContext.OrderRequests
                .AsNoTracking()
                .Select(o => new { o.OrderRequestId, o.Status })
                .FirstOrDefaultAsync(oe => oe.OrderRequestId == orderRequestId);

            if (orderRequest == null)
            {
                return null;
            }

            bool isLocked = 
                orderRequest.Status == OrderStatus.Completed || 
                orderRequest.Status == OrderStatus.Cancelled;

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
                IsLocked = isLocked,
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
            bool isLocked = await IsOrderLockedAsync(model.OrderRequestId);

            if (isLocked)
            {
                return false;
            }

            if (model.Quantity < 1 || model.Quantity > 100)
            {
                return false;
            }

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
            OrderRequest? order = await dbContext.OrderRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderRequestId == orderRequestId);

            if (order == null)
                return null;

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

        public async Task<bool> UpdateItemQuantityAsync(int orderItemId, int requestedQuantity)
        {
            if (requestedQuantity <= 0 || requestedQuantity > 100)
            {
                return false;
            }

            OrderItem? orderItemToUpdate = await dbContext
                .OrderItems
                .FirstOrDefaultAsync(i => i.OrderItemId == orderItemId);


            if (orderItemToUpdate == null)
            {
                return false;
            }

            bool isLocked = await IsOrderLockedAsync(orderItemToUpdate.OrderRequestId);

            if (isLocked)
            {
                return false;
            }

            orderItemToUpdate.Quantity = requestedQuantity;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveItemAsync(int orderItemId)
        {
            OrderItem? orderItemToRemove = await dbContext
                .OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);

            if (orderItemToRemove == null)
            {
                return false;
            }

            bool isLocked = await IsOrderLockedAsync(orderItemToRemove.OrderRequestId);

            if (isLocked)
            {
                return false;
            }

            dbContext.OrderItems.Remove(orderItemToRemove);

            await dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<OrderListViewModel> GetAllForManagementAsync(int? statusFilter, string? searchTerm)
        {
            IQueryable<OrderRequest> orderRequestsQuery = dbContext
                .OrderRequests
                .AsNoTracking();

            if(statusFilter.HasValue)
            {
                orderRequestsQuery = orderRequestsQuery.Where(or => (int)or.Status == statusFilter.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();

                bool isNumber = int.TryParse(term, out int parsedOrderId);

                orderRequestsQuery = orderRequestsQuery.Where(or =>
                or.CustomerName.Contains(term) ||
                or.PhoneNumber.Contains(term) ||
                (isNumber && or.OrderRequestId == parsedOrderId));
            }

            List<OrderListItemViewModel> ordersForManagement = await orderRequestsQuery
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

            return new OrderListViewModel
            {
                Orders = ordersForManagement,
                StatusFilter = statusFilter,
                SearchTerm = searchTerm
            };
        }

        public async Task<bool> UpdateStatusAsync(int orderRequestId, OrderStatus newStatus)
        {
            OrderRequest? orderRequestToUpdate = await dbContext
                .OrderRequests
                .FirstOrDefaultAsync(or => or.OrderRequestId == orderRequestId);

            if (orderRequestToUpdate == null)
            {
                return false;
            }

            if (orderRequestToUpdate.Status == OrderStatus.Completed ||
               orderRequestToUpdate.Status == OrderStatus.Cancelled)
            {
                return false;
            }

            bool isValidTransition =
                (orderRequestToUpdate.Status == OrderStatus.Pending && (newStatus == OrderStatus.Confirmed || newStatus == OrderStatus.Cancelled)) ||
                (orderRequestToUpdate.Status == OrderStatus.Confirmed && (newStatus == OrderStatus.Ready || newStatus == OrderStatus.Cancelled)) ||
                (orderRequestToUpdate.Status == OrderStatus.Ready && (newStatus == OrderStatus.Completed || newStatus == OrderStatus.Cancelled));

            if (!isValidTransition)
            {
                return false;
            }

            orderRequestToUpdate.Status = newStatus;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<OrderDetailsViewModel?> GetDetailsForManagementAsync(int orderRequestId)
        {
            OrderRequest? order = await dbContext.OrderRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderRequestId == orderRequestId);

            if (order == null)
            {
                return null;
            }

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

            decimal totalPrice = items.Sum(i => i.RowTotal);

            return new OrderDetailsViewModel
            {
                OrderRequestId = order.OrderRequestId,
                CustomerName = order.CustomerName,
                PhoneNumber = order.PhoneNumber,
                CreatedOn = order.CreatedOn,
                PickupDate = order.PickupDate,
                StatusValue = (int)order.Status,
                Status = order.Status.ToString(),
                Notes = order.Notes,
                Items = items,
                TotalPrice = totalPrice
            };
        }
        private async Task<bool> IsOrderLockedAsync(int orderRequestId)
        {
            return await dbContext.OrderRequests
                .AsNoTracking()
                .AnyAsync(o => o.OrderRequestId == orderRequestId &&
                          (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled));
        }

        public async Task<bool> UserCanAccessOrderAsync(int orderRequestId, string userId, bool isAdmin)
        {
            if(isAdmin)
            {
                return true;
            }

            return await dbContext
                .OrderRequests
                .AsNoTracking()
                .AnyAsync(or => or.OrderRequestId == orderRequestId && or.UserId == userId);
        }
    }
}
