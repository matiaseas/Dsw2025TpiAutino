namespace Dsw2025Tpi.Application.Dtos
{
    /// DTO para la creación de un producto.
    public record ProductCreateDto(
        string Sku,
        string InternalCode,
        string Name,
        string Description,
        decimal CurrentUnitPrice,
        int StockQuantity,
        bool IsActive = true
    );

    /// DTO para la actualización de un producto.
    public record ProductUpdateDto(
        Guid Id,
        string Sku,
        string InternalCode,
        string Name,
        string Description,
        decimal CurrentUnitPrice,
        int StockQuantity,
        bool IsActive
    );

    /// DTO de respuesta de un producto.
    public record ProductDto(
        Guid Id,
        string Sku,
        string InternalCode,
        string Name,
        string Description,
        decimal CurrentUnitPrice,
        int StockQuantity,
        bool IsActive
    );
}
