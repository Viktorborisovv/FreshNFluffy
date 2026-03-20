namespace FreshNFluffy.Areas.Admin.Controllers
{
    using FreshNFluffy.Data.Models.Enum;
    using FreshNFluffy.Services.Interfaces;
    using FreshNFluffy.ViewModels.Orders.Management;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;


    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int? statusFilter, string? searchTerm)
        {
            OrderListViewModel model = await orderService
                .GetAllForManagementAsync(statusFilter, searchTerm);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            OrderDetailsViewModel? model = await orderService
                .GetDetailsForManagementAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderRequestId, int newStatus)
        {
            bool isUpdated = await orderService.UpdateStatusAsync(orderRequestId, (OrderStatus)newStatus);

            if (!isUpdated)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}
