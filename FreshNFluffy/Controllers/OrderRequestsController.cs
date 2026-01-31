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
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AddItems), new { id = model.NewItem.OrderRequestId });
            }

            bool addedItem = await orderService.AddItemAsync(model.NewItem);

            if (!addedItem)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(AddItems), new { id = model.NewItem.OrderRequestId });
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
                return BadRequest();
            }

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
                return BadRequest();
            }

            return RedirectToAction(nameof(AddItems), new { id = orderRequestId });
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            OrderListViewModel model = await orderService.GetAllForManagementAsync();

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
