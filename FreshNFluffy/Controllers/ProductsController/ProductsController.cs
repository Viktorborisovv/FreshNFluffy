namespace FreshNFluffy.Controllers
{
    using FreshNFluffy.Services.Interfaces;
    using FreshNFluffy.ViewModels.Products;
    using Microsoft.AspNetCore.Mvc;
    public class ProductsController : Controller
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ProductQueryViewModel query)
        {
            ProductQueryViewModel model = await productService.GetAllAsync(query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await productService.GetDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await productService.GetCreateFormAsync();

            return View(model);
        }

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await productService.GetEditFormAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductFormViewModel model)
        {
            if(!ModelState.IsValid)
            {
                var fresh = await productService.GetCreateFormAsync();

                model.Categories = fresh.Categories;

                return View(model);
            }

            bool productUpdated = await productService.EditAsync(model);

            if (!productUpdated)
                return BadRequest();

            return RedirectToAction(nameof(Details), new { id = model.ProductId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await productService.GetDeleteAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool productDeleted = await productService.DeleteAsync(id);

            if(!productDeleted)
                return BadRequest();

            return RedirectToAction(nameof(Index));
        }
    }
}
