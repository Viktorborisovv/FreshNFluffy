namespace FreshNFluffy.Services
{
    using FreshNFluffy.Data;
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Models.Enum;
    using FreshNFluffy.Data.Repository.Contracts;
    using FreshNFluffy.Services.Interfaces;

    using FreshNFluffy.ViewModels.Categories;
    using FreshNFluffy.ViewModels.Products;
    using FreshNFluffy.ViewModels.Reviews;
    using Microsoft.EntityFrameworkCore;

    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<ProductQueryViewModel> GetAllAsync(ProductQueryViewModel query)
        {
            query.Categories = await productRepository.GetAllCategoriesAsync();

            IQueryable<Product> productsQuery = productRepository.GetAllAsQueryable();

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

            if (query.CurrentPage <= 0)
            {
                query.CurrentPage = 1;
            }

            query.TotalProducts = await productsQuery.CountAsync();

            query.Products = await productsQuery
                .OrderBy(p => p.Category.Name)
                .ThenBy(p => p.Name)
                .Skip((query.CurrentPage - 1) * query.ProductsPerPage)
                .Take(query.ProductsPerPage)
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
            Product? product = await productRepository.GetByIdWithCategoryAndReviewsAsync(id);

            if (product == null)
            {
                return null;
            }

            return new ProductDetailsViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category.Name,
                NutritionText = product.NutritionTypes.ToString(),

                Reviews = product.Reviews
                        .OrderByDescending(r => r.CreatedOn)
                        .Select(r => new ReviewListItemViewModel
                        {
                            Rating = r.Rating,
                            Comment = r.Comment,
                            UserName = r.User.UserName!,
                            CreatedOn = r.CreatedOn
                        })
                        .ToList(),

                NewReview = new CreateReviewViewModel
                {
                    ProductId = product.ProductId
                }
            };
        }

        //Create action
        public async Task<ProductFormViewModel> GetCreateFormAsync()
        {
            return new ProductFormViewModel
            {
                Categories = await productRepository.GetAllCategoriesAsync()
            };
        }
        public async Task<int> CreateAsync(ProductFormViewModel model)
        {
            bool categoryExists = await productRepository.CategoryExistsAsnyc(model.CategoryId);

            if (!categoryExists)
            {
                return 0;
            }

            Product product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                NutritionTypes = (NutritionTypes)model.NutritionTypes
            };

            await productRepository.AddAsync(product);
            await productRepository.SaveChangesAsync();

            return product.ProductId;
        }

        //Edit action
        public async Task<ProductFormViewModel?> GetEditFormAsync(int id)
        {
            Product? product = await productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return new ProductFormViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                NutritionTypes = (int)product.NutritionTypes,
                Categories = await productRepository.GetAllCategoriesAsync()
            };
        }
        public async Task<bool> EditAsync(ProductFormViewModel model)
        {
            if(!model.ProductId.HasValue)
            {
                return false;
            }

            Product? product = await productRepository.GetByIdAsync(model.ProductId.Value);

            if (product == null)
            {
                return false;
            }

            bool categoryExists = await productRepository.CategoryExistsAsnyc(model.CategoryId);

            if (!categoryExists)
            {
                return false;
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.NutritionTypes = (NutritionTypes)model.NutritionTypes;

            await productRepository.SaveChangesAsync();
            return true;
        }

        //Delete action
        public async Task<ProductDetailsViewModel?> GetDeleteAsync(int id)
        {
            Product? product = await productRepository.GetByIdWithCategoryAsync(id);

            if (product == null)
            {
                return null;
            }

            return new ProductDetailsViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category.Name,
                NutritionText = product.NutritionTypes.ToString()
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Product? product = await productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return false;
            }
                
            bool isUsedInOrders = await productRepository.ProductIsUsedInOrdersAsync(id);

            if (isUsedInOrders)
            {
                return false;
            }

            productRepository.Remove(product);
            await productRepository.SaveChangesAsync();

            return true;
        }
    }
}
