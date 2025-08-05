using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services
{
    public class ProductsManagementService
    {
        private readonly IRepository _repository;

        public ProductsManagementService(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        public async Task<ProductDto> CreateProductAsync(ProductCreateDto dto)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(dto.Sku) ||
                string.IsNullOrWhiteSpace(dto.InternalCode) ||
                string.IsNullOrWhiteSpace(dto.Name) ||
                dto.CurrentUnitPrice <= 0 ||
                dto.StockQuantity < 0)
            {
                throw new ArgumentException("Datos de producto inválidos.");
            }

            // Verificar SKU único
            var existing = await _repository.First<Product>(p => p.Sku == dto.Sku);
            if (existing != null)
                throw new ArgumentException($"Ya existe un producto con SKU {dto.Sku}.");

            // Crear entidad
            var product = new Product(
                dto.Sku,
                dto.InternalCode,
                dto.Name,
                dto.Description,
                dto.CurrentUnitPrice,
                dto.StockQuantity
            );

            // Persistir
            await _repository.Add(product);

            // Mapear a DTO
            return new ProductDto(
                product.Id,
                product.Sku!,
                product.InternalCode!,
                product.Name!,
                product.Description!,
                product.CurrentUnitPrice,
                product.StockQuantity,
                product.IsActive
            );
        }

        /// <summary>
        /// Obtiene todos los productos activos.
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _repository.GetAll<Product>();
            return (products ?? Array.Empty<Product>())
                .Where(p => p.IsActive)
                .Select(p => new ProductDto(
                    p.Id,
                    p.Sku!,
                    p.InternalCode!,
                    p.Name!,
                    p.Description!,
                    p.CurrentUnitPrice,
                    p.StockQuantity,
                    p.IsActive))
                .ToList();
        }

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var p = await _repository.GetById<Product>(id);
            if (p == null || !p.IsActive)
                return null;

            return new ProductDto(
                p.Id,
                p.Sku!,
                p.InternalCode!,
                p.Name!,
                p.Description!,
                p.CurrentUnitPrice,
                p.StockQuantity,
                p.IsActive);
        }

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        public async Task<ProductDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto)
        {
            var p = await _repository.GetById<Product>(id);
            if (p == null)
                return null;

            // Actualizar campos
            p.Sku = dto.Sku;
            p.InternalCode = dto.InternalCode;
            p.Name = dto.Name;
            p.Description = dto.Description;
            p.CurrentUnitPrice = dto.CurrentUnitPrice;
            p.StockQuantity = dto.StockQuantity;
            p.IsActive = dto.IsActive;

            await _repository.Update(p);

            return new ProductDto(
                p.Id,
                p.Sku!,
                p.InternalCode!,
                p.Name!,
                p.Description!,
                p.CurrentUnitPrice,
                p.StockQuantity,
                p.IsActive);
        }

        /// <summary>
        /// Deshabilita (soft delete) un producto.
        /// </summary>
        public async Task<bool> DisableProductAsync(Guid id)
        {
            var p = await _repository.GetById<Product>(id);
            if (p == null)
                return false;

            p.IsActive = false;
            await _repository.Update(p);
            return true;
        }
    }
}
