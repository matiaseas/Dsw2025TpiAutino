using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services;

//aqui se podria poner una interfaz como IProductsManagementService
public class ProductsManagementService
{
	private readonly IRepository _repository;

	public ProductsManagementService(IRepository repository)
	{
		_repository = repository;
	}

	//public async Task<ProductModel.Response?> GetProductById(Guid id)

	//referenciaD
	public async Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request)
	{
		/*
		if (string.IsNullOrWhiteSpace(request.Sku) ||
			string.IsNullOrWhiteSpace(request.InternalCode) ||
			string.IsNullOrWhiteSpace(request.Description) || string.IsNullOrWhiteSpace(request.Name) ||
			request.StockQuantity < 0)
		{
			throw new ArgumentException("Valores para el producto no validos");
		}
		if (request.CurrentUnitPrice <= 0) throw new PriceNullException("El precio del producto no puede ser cero o menor."); var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
		
		if (exist != null) throw new DuplicatedEntityException($"Ya existe un producto con el Sku {request.Sku}");
		*/



		var product = new Product(
			request.Sku, 
			request.InternalCode, 
			request.Name, 
			request.Description, 
			request.CurrentUnitPrice, 
			request.StockQuantity);

		//if (product.Sku is null)
		//	throw new InvalidOperationException("El SKU no puede ser nulo.");
		if (string.IsNullOrWhiteSpace(request.Sku) ||
			string.IsNullOrWhiteSpace(request.InternalCode) ||
			string.IsNullOrWhiteSpace(request.Description) ||
			string.IsNullOrWhiteSpace(request.Name) ||
			request.StockQuantity < 0)
		{
			throw new ArgumentException("Valores para el producto no validos");
		}

			await _repository.Add(product);
		return new ProductModel.ProductResponse(product.Id, product.Sku, product.Name, product.CurrentUnitPrice, product.InternalCode, product.Description, product.StockQuantity);
	}
}

