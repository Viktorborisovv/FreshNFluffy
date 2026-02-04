namespace FreshNFluffy.Controllers
{
    using FreshNFluffy.Data.Models.Enum;

    using FreshNFluffy.Services.Interfaces;

    using FreshNFluffy.ViewModels.Orders;
    using FreshNFluffy.ViewModels.Orders.Management;

    using Microsoft.AspNetCore.Mvc;

    public class OrderRequestsController : Controller
    {
        private readonly IOrderService orderService;

        public OrderRequestsController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            OrderCreateViewModel model = new OrderCreateViewModel
            {
                PickupDate = DateTime.Today.AddDays(1)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddItems(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            AddOrderItemViewModel? model = await orderService.GetAddItemsFormAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            if (model.IsLocked)
            {
                TempData["Error"] = "This order is locked (Completed or Cancelled). You cannot edit items.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int orderId = await orderService.CreateOrderAsync(model);

            if (orderId <= 0)
            {
                ModelState.AddModelError("", "Could not create order");
                return View(model);
            }

            return RedirectToAction(nameof(AddItems), new { id = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(AddOrderItemViewModel model)
        {
            int orderRequestId = model.NewItem.OrderRequestId;

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please select a product and enter a valid quantity (1–100).";
                return RedirectToAction(nameof(AddItems), new { id = model.NewItem.OrderRequestId });
            }

            bool isItemAddedSuccessfully = await orderService.AddItemAsync(model.NewItem);

            if(!isItemAddedSuccessfully)
            {
                TempData["Error"] = "Cannot add item. The order may be locked or the selected product is invalid.";
                return RedirectToAction(nameof(AddItems), new {id = orderRequestId});
            }

            TempData["Success"] = "Item added successfully";
            return RedirectToAction(nameof(AddItems), new { id = orderRequestId });
        }

        [HttpGet]
        public async Task<IActionResult> Summary(int id)
        {
            OrderSummaryViewModel? model = await orderService.GetSummaryAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItemQuantity(int orderItemId, int orderRequestId, int newQuantity)
        {
            bool isQuantityUpdatedSuccessfully =
                await orderService.UpdateItemQuantityAsync(orderItemId, newQuantity);

            if (!isQuantityUpdatedSuccessfully)
            {
                TempData["Error"] = "Cannot update quantity. The order may be locked or the quantity is invalid.";
                return RedirectToAction(nameof(AddItems), new { id = orderRequestId });
            }

            TempData["Success"] = "Quantity updated successfully.";
            return RedirectToAction(nameof(AddItems), new { id = orderRequestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int orderItemId, int orderRequestId)
        {
            bool isItemRemovedSuccessfully =
                await orderService.RemoveItemAsync(orderItemId);

            if (!isItemRemovedSuccessfully)
            {
                TempData["Error"] = "Cannot remove item. The order may be locked or the item no longer exists.";
                return RedirectToAction(nameof(AddItems), new { id = orderRequestId });
            }

            TempData["Success"] = "Item removed successfully.";
            return RedirectToAction(nameof(AddItems), new { id = orderRequestId });
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int? statusFilter, string? searchTerm)
        {
            OrderListViewModel model = await orderService.GetAllForManagementAsync(statusFilter, searchTerm);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderRequestId, int newStatus)
        {
            if (orderRequestId <= 0)
            {
                return BadRequest();
            }

            if(!Enum.IsDefined(typeof(OrderStatus), newStatus))
            {
                return BadRequest();
            }

            OrderStatus requestedStatus = (OrderStatus)newStatus;

            bool isStatusUpdatedSuccessfully =
                await orderService.UpdateStatusAsync(orderRequestId, requestedStatus);

            if (!isStatusUpdatedSuccessfully)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if(id <= 0)
            {
                return BadRequest();
            }

            OrderDetailsViewModel? model = await orderService.GetDetailsForManagementAsync(id);
           
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
