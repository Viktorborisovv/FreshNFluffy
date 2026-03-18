namespace FreshNFluffy.Controllers
{
    using FreshNFluffy.Services.Interfaces;
    using FreshNFluffy.ViewModels.Products;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    public class ProductsController : Controller
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ProductQueryViewModel query)
        {
            ProductQueryViewModel model = await productService.GetAllAsync(query);

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ProductDetailsViewModel? model = await productService.GetDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ProductFormViewModel model = await productService.GetCreateFormAsync();

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await productService.GetCreateFormAsync();

                return View(model);
            }

            int newId = await productService.CreateAsync(model);

            if (newId == 0)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Invalid category");

                model = await productService.GetCreateFormAsync();

                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = newId });
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ProductFormViewModel? model = await productService.GetEditFormAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductFormViewModel model)
        {
            if(!ModelState.IsValid)
            {
                ProductFormViewModel formModelWithCategories = await productService.GetCreateFormAsync();

                model.Categories = formModelWithCategories.Categories;

                return View(model);
            }

            bool productUpdated = await productService.EditAsync(model);

            if (!productUpdated)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index), new { id = model.ProductId });
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ProductDetailsViewModel? model = await productService.GetDeleteAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool productDeleted = await productService.DeleteAsync(id);


            if (!productDeleted)
            {
                TempData["Error"] = "This product cannot be deleted because it is used in existing orders.";

                return RedirectToAction(nameof(Details), new { id });
            }

            if(id <= 0)
            {
                return BadRequest();
            }

            TempData["Success"] = "Product deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
