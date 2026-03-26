namespace FreshNFluffy.Services
{
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Models.Enum;
    using FreshNFluffy.Data.Repository.Contracts;

    using FreshNFluffy.Services.Interfaces;
    using FreshNFluffy.ViewModels.Orders;
    using FreshNFluffy.ViewModels.Orders.Management;

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
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

            await orderRepository.AddOrderRequestAsync(order);
            await orderRepository.SaveChangesAsync();

            return order.OrderRequestId;
        }
        public async Task<AddOrderItemViewModel?> GetAddItemsFormAsync(int orderRequestId)
        {
            OrderRequest? orderRequest = await orderRepository.GetOrderRequestByIdAsNoTrackingAsync(orderRequestId);

            if (orderRequest == null)
            {
                return null;
            }

            bool isLocked = 
                orderRequest.Status == OrderStatus.Completed || 
                orderRequest.Status == OrderStatus.Cancelled;

            IEnumerable<ProductSelectViewModel> products = await orderRepository.GetProductsForOrderAsync();

            IEnumerable<OrderItemRowViewModel> items = await orderRepository.GetOrderItemsByOrderRequestIdAsync(orderRequestId);

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
            bool isLocked = await orderRepository.IsOrderLockedAsync(model.OrderRequestId);

            if (isLocked)
            {
                return false;
            }

            if (model.Quantity < 1 || model.Quantity > 100)
            {
                return false;
            }

            bool orderExists = await orderRepository.OrderRequestExistsAsync(model.OrderRequestId);

            if (!orderExists)
            {
                return false;
            }

            Product? product = await orderRepository.GetProductByIdAsync(model.ProductId);

            if (product == null)
            {
                return false;
            }

            OrderItem? existingItem = await orderRepository.
                GetOrderItemByOrderAndProductAsync(model.OrderRequestId, model.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += model.Quantity;

                await orderRepository.SaveChangesAsync();
                return true;
            }

            OrderItem newItem = new OrderItem
            {
                OrderRequestId = model.OrderRequestId,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                UnitPrice = product.Price
            };

            await orderRepository.AddOrderItemAsync(newItem);
            await orderRepository.SaveChangesAsync();

            return true;
        }

        public async Task<OrderSummaryViewModel?> GetSummaryAsync(int orderRequestId)
        {
            OrderRequest? order = await orderRepository.GetOrderRequestByIdAsNoTrackingAsync(orderRequestId);

            if (order == null)
            {
                return null;
            }

            List<OrderItemRowViewModel> items = (await orderRepository
                .GetOrderItemsByOrderRequestIdAsync(orderRequestId))
                .ToList();

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

            OrderItem? orderItemToUpdate = await orderRepository.GetOrderItemByIdAsync(orderItemId);

            if (orderItemToUpdate == null)
            {
                return false;
            }

            bool isLocked = await orderRepository.IsOrderLockedAsync(orderItemToUpdate.OrderRequestId);

            if (isLocked)
            {
                return false;
            }

            orderItemToUpdate.Quantity = requestedQuantity;
            await orderRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveItemAsync(int orderItemId)
        {
            OrderItem? orderItemToRemove = await orderRepository.GetOrderItemByIdAsync(orderItemId);

            if (orderItemToRemove == null)
            {
                return false;
            }

            bool isLocked = await orderRepository.IsOrderLockedAsync(orderItemToRemove.OrderRequestId);

            if (isLocked)
            {
                return false;
            }

            orderRepository.RemoveOrderItem(orderItemToRemove);
            await orderRepository.SaveChangesAsync();

            return true;
        }
        public async Task<OrderListViewModel> GetAllForManagementAsync(int? statusFilter, string? searchTerm)
        {
            List<OrderListItemViewModel> ordersForManagement = (await orderRepository
                .GetOrdersForManagementAsync(statusFilter, searchTerm))
                .ToList();

            return new OrderListViewModel
            {
                Orders = ordersForManagement,
                StatusFilter = statusFilter,
                SearchTerm = searchTerm
            };
        }

        public async Task<bool> UpdateStatusAsync(int orderRequestId, OrderStatus newStatus)
        {
            OrderRequest? orderRequestToUpdate = await orderRepository.GetOrderRequestByIdAsync(orderRequestId);

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
            await orderRepository.SaveChangesAsync();

            return true;
        }

        public async Task<OrderDetailsViewModel?> GetDetailsForManagementAsync(int orderRequestId)
        {
            OrderRequest? order = await orderRepository.GetOrderRequestByIdAsNoTrackingAsync(orderRequestId);

            if (order == null)
            {
                return null;
            }

            List<OrderItemRowViewModel> items = (await orderRepository
                .GetOrderItemsByOrderRequestIdAsync(orderRequestId))
                .ToList();

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

        public async Task<bool> UserCanAccessOrderAsync(int orderRequestId, string userId, bool isAdmin)
        {
            if(isAdmin)
            {
                return true;
            }

            return await orderRepository.UserOwnsOrderAsync(orderRequestId, userId);
        }
    }
}
