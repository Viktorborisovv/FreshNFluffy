namespace FreshNFluffy.Controllers
{
    using FreshNFluffy.Services.Interfaces;
    using FreshNFluffy.ViewModels.Orders;

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

            var model = await orderService.GetAddItemsFormAsync(id);

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
            var model = await orderService.GetSummaryAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
