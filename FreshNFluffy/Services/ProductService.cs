namespace FreshNFluffy.Services
{
    using FreshNFluffy.Data;
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Models.Enum;

    using FreshNFluffy.Services.Interfaces;

    using FreshNFluffy.ViewModels.Categories;
    using FreshNFluffy.ViewModels.Products;

    using Microsoft.EntityFrameworkCore;

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

            IQueryable<Product> productsQuery = dbContext.Products
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
                    NutritionText = p.NutritionTypes.ToString()
                })
                .FirstOrDefaultAsync();
        }

        //Create action
        public async Task<ProductFormViewModel> GetCreateFormAsync()
        {
            List<CategorySelectViewModel> categories = await dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategorySelectViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                })
                .ToListAsync();

            return new ProductFormViewModel
            {
                Categories = categories
            };
        }
        public async Task<int> CreateAsync(ProductFormViewModel model)
        {
            bool categoryExists = await dbContext.Categories
                .AsNoTracking()
                .AnyAsync(c => c.CategoryId == model.CategoryId);

            if (!categoryExists)
                return 0;

            Product product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                NutritionTypes = (NutritionTypes)model.NutritionTypes
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            return product.ProductId;
        }

        //Edit action
        public async Task<ProductFormViewModel?> GetEditFormAsync(int id)
        {
            Product? product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return null;
            }

            List<CategorySelectViewModel> categories = await dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategorySelectViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                })
                .ToListAsync();

            return new ProductFormViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                NutritionTypes = (int)product.NutritionTypes,
                Categories = categories
            };
        }
        public async Task<bool> EditAsync(ProductFormViewModel model)
        {
            if(model.ProductId == null)
                return false;

            Product? product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId.Value);

            if(product == null)
                return false;

            bool categoryExists = await dbContext.Categories
                .AsNoTracking()
                .AnyAsync(c => c.CategoryId == model.CategoryId);

            if (!categoryExists)
                return false;

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.NutritionTypes = (NutritionTypes)model.NutritionTypes;

            await dbContext.SaveChangesAsync();
            return true;
        }

        //Delete action
        public async Task<ProductDetailsViewModel?> GetDeleteAsync(int id)
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
                    NutritionText = p.NutritionTypes.ToString()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Product? product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return false;

            dbContext.Products.Remove(product);

            try
            {
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
