namespace FreshNFluffy.Controllers.ProductsController
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

            if(model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
