namespace Dsw2025Tpi.Application.DTOs
{

    /// Parametros de consulta para paginación de órdenes.
    public record OrderQueryParameters(int PageNumber = 1, int PageSize = 10);

    /// DTO para la creación de una orden.
    public record OrderCreateDto(
        Guid CustomerId,
        IEnumerable<OrderItemDto> Items
    );

    /// Item de orden en la creación.
    public record OrderItemDto(
        Guid ProductId,
        int Quantity
    );

    /// DTO para actualización de estado de orden.
    public record OrderStatusUpdateDto(string Status);

    /// DTO de respuesta de una orden.
    public record OrderDto(
        Guid Id,
        Guid CustomerId,
        DateTime CreatedAt,
        decimal Subtotal,
        decimal Total,
        string Status,
        IEnumerable<OrderItemDetailDto> Items
    );

    /// Detalle de item en la respuesta de orden.
    public record OrderItemDetailDto(
        Guid ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal
    );

    /// Resultado paginado genérico.
    public record PagedResult<T>(
        IEnumerable<T> Items,
        int TotalCount,
        int PageNumber,
        int PageSize
    );
}
