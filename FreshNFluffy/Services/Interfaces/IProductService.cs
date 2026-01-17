using FreshNFluffy.ViewModels.Products;

namespace FreshNFluffy.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductQueryViewModel> GetAllAsync(ProductQueryViewModel query);
        Task<ProductDetailsViewModel?> GetDetailsAsync(int id);
    }
}
