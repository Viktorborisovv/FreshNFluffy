namespace FreshNFluffy.Services.Interfaces
{
    using FreshNFluffy.ViewModels.Products;
    public interface IProductService
    {
        Task<ProductQueryViewModel> GetAllAsync(ProductQueryViewModel query);
        Task<ProductDetailsViewModel?> GetDetailsAsync(int id);

        Task<ProductFormViewModel> GetCreateFormAsync();
        Task<int> CreateAsync(ProductFormViewModel model);

        Task<ProductFormViewModel?> GetEditFormAsync(int id);
        Task<bool> EditAsync(ProductFormViewModel model);

        Task<ProductDetailsViewModel?> GetDeleteAsync(int id);
        Task<bool> DeleteAsync(int id);

    }
}
