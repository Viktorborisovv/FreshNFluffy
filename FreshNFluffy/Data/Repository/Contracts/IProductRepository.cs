namespace FreshNFluffy.Data.Repository.Contracts
{
    using FreshNFluffy.Data.Models;
    using FreshNFluffy.ViewModels.Categories;
    public interface IProductRepository
    {
        Task<IEnumerable<CategorySelectViewModel>> GetAllCategoriesAsync();
        IQueryable<Product> GetAllAsQueryable();
        Task<Product?> GetByIdAsync(int id);

        Task<Product?> GetByIdWithCategoryAsync(int id);

        Task<Product?> GetByIdWithCategoryAndReviewsAsync(int id);

        Task<bool> CategoryExistsAsnyc(int categoryId);

        Task<bool> ProductExistsAsync(int id);

        Task<bool> ProductIsUsedInOrdersAsync(int productId);

        Task AddAsync(Product product);

        void Remove(Product product);

        Task<int> SaveChangesAsync();
    }
}
