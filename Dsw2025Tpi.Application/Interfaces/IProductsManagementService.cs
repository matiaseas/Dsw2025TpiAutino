using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IProductsManagementService
    {
        Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request);
        Task<ProductModel.ProductResponse?> GetProductById(Guid id);
        Task<IEnumerable<ProductModel.ProductResponse>?> GetProducts();
        Task<ProductModel.ProductResponse?> ToggleStatus(Guid id);
        Task<ProductModel.ProductResponse?> Update(Guid id, ProductModel.ProductRequest request);
    }
}