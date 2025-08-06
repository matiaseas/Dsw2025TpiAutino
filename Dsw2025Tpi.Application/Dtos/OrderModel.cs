using Dsw2025Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderModel
{
    public record OrderRequest(
        Guid CustomerId,
        string ShippingAddress,
        string BillingAddress,
        string Notes,
        List<OrderItemRequest> OrderItems
        );

    public record OrderResponse(
        Guid Id,
        Guid CustomerId,
        string ShippingAddress,
        string BillingAddress,
        string Notes,
        List<OrderItemResponse> OrderItems,
        string OrderStatus
        );

    public record OrderItemRequest( 
        Guid ProductId,
        int Quantity
    );

    public record OrderItemResponse(
        Guid ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        decimal SubTotal
    );

    public record OrderStatusRequest(
        string OrderStatus);
}