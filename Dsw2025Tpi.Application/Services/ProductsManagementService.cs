using Azure.Core;
using Dsw2025Ej15.Application.Exceptions;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dsw2025Tpi.Application.Dtos.ProductModel;

namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService : IProductsManagementService
{
    private readonly IRepository _repository;

    public ProductsManagementService(IRepository repository)
    {
        _repository = repository;

    }
    public void ValidateRequest(ProductRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Sku))
            errors.Add("El SKU no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("El nombre no puede estar vacío.");
        if (request.CurrentUnitPrice <= 0)
            errors.Add("El precio unitario debe ser mayor a 0.");
        if (request.StockQuantity < 0)
            errors.Add("La cantidad en stock no puede ser negativa.");
        if (errors.Any())
            throw new ArgumentException(string.Join(" | ", errors));
    }

    private ProductModel.ProductResponse MapToResponse(Product product)
    {
        return new ProductModel.ProductResponse(
            product.Id,
            product.Sku,
            product.InternalCode,
            product.Name,
            product.Description,
            product.CurrentUnitPrice,
            product.StockQuantity,
            product.IsActive);
    }

    public async Task<ProductModel.ProductResponse?> GetProductById(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new EntityNotFoundException($"No existe el producto con Id: {id}");
        return product != null ? new ProductModel.ProductResponse(
            product.Id,
            product.Sku,
            product.InternalCode,
            product.Name,
            product.Description,
            product.CurrentUnitPrice,
            product.StockQuantity,
            product.IsActive)
            : null;
    }

    public async Task<IEnumerable<ProductModel.ProductResponse>?> GetProducts()
    {
        var products = await _repository.GetFiltered<Product>(p => p.IsActive);
        if(products == null || !products.Any()) 
            throw new EntityNotFoundException("No hay productos cargados");
        return products?.Select(MapToResponse);
    }

    public async Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request)
    {
        ValidateRequest(request);

        var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
        if (exist != null) 
            throw new DuplicatedEntityException($"Ya existe un producto con el Sku {request.Sku}");

        var product = new Product(request.Sku,
            request.InternalCode,
            request.Name,
            request.Description,
            request.CurrentUnitPrice,
            request.StockQuantity);

        await _repository.Add(product);

        return MapToResponse(product);
    }

    public async Task<ProductModel.ProductResponse?> Update(Guid id, ProductModel.ProductRequest request)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new EntityNotFoundException($"No se encontró un producto con el ID {id}");

        ValidateRequest(request);

        if(request.Sku != product.Sku)
        {
            var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
            if (exist != null) 
                throw new DuplicatedEntityException($"Ya existe un producto con el Sku {request.Sku}");
        }

        product.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.Name;
        product.Description = request.Description;
        product.CurrentUnitPrice = request.CurrentUnitPrice;
        product.StockQuantity = request.StockQuantity;

        var updatedProduct = await _repository.Update(product);

        return MapToResponse(product);
    }

    public async Task<ProductModel.ProductResponse?> ToggleStatus(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new EntityNotFoundException($"No se encontró un producto con el ID {id}");

        product.IsActive = false;

        var updatedProduct = await _repository.Update(product);

        return MapToResponse(product);
    }
}
