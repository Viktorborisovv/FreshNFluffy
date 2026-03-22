namespace FreshNFluffy.Data.Repository
{
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.Data.Repository.Contracts;
    using FreshNFluffy.ViewModels.Categories;
    using Microsoft.EntityFrameworkCore;

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<CategorySelectViewModel>> GetAllCategoriesAsync()
        {
            return await dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategorySelectViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();
        }
        public IQueryable<Product> GetAllAsQueryable()
        {
            return dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category);
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }
        public async Task<Product?> GetByIdWithCategoryAsync(int id)
        {
            return await dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }
        public async Task<Product?> GetByIdWithCategoryAndReviewsAsync(int id)
        {
            return await dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<bool> CategoryExistsAsnyc(int categoryId)
        {
            return await dbContext.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await dbContext.Products.AnyAsync(p => p.ProductId == id);
        }

        public async Task<bool> ProductIsUsedInOrdersAsync(int productId)
        {
            return await dbContext.OrderItems.AnyAsync(oi => oi.ProductId == productId);
        }
        public async Task AddAsync(Product product)
        {
            await dbContext.Products.AddAsync(product);
        }

        public void Remove(Product product)
        {
            dbContext.Products.Remove(product);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
