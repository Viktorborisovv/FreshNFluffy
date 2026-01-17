using FreshNFluffy.Data;
using FreshNFluffy.Data.Models.Enum;
using FreshNFluffy.Services.Interfaces;
using FreshNFluffy.ViewModels.Categories;
using FreshNFluffy.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace FreshNFluffy.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext dbContext;

        public ProductService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ProductQueryViewModel> GetAllAsync(ProductQueryViewModel query)
        {
            query.Categories = await dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategorySelectViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();

            IQueryable<Data.Models.Product> productsQuery = dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category);

            if (query.CategoryId.HasValue)
            {
                productsQuery = productsQuery
                       .Where(p => p.CategoryId == query.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim();
                productsQuery = productsQuery
                       .Where(p => p.Name.Contains(search)
                       || (p.Description != null
                       && p.Description.Contains(search)));
            }

            if (query.NutritionTypes.HasValue && query.NutritionTypes.Value != 0)
            {
                int selected = query.NutritionTypes.Value;
                productsQuery = productsQuery
                       .Where(p => (((int)p.NutritionTypes) & selected) == selected);
            }

            query.Products = await productsQuery
                .OrderBy(p => p.Category.Name)
                .ThenBy(p => p.Name)
                .Select(p => new ProductListItemViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    NutritionTypes = (int)p.NutritionTypes
                })
                .ToListAsync();

            return query;
        }

        public async Task<ProductDetailsViewModel?> GetDetailsAsync(int id)
        {
            return await dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDetailsViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category.Name,
                    NutritionText = ((NutritionTypes)p.NutritionTypes).ToString()
                })
                .FirstOrDefaultAsync();
        }
    }
}
