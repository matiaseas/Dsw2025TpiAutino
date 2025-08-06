namespace Dsw2025Tpi.Domain.Entities;

public class OrderItem : EntityBase
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public OrderItem() { }

    public OrderItem(int quantity, Product product, Order order)
    {
        if (!product.HasSufficientStock(quantity))
            throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}");

        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        UnitPrice = product.CurrentUnitPrice;
        SubTotal = CalculateSubTotal();
        OrderId = Order.Id;
    }

    public decimal CalculateSubTotal() => UnitPrice * Quantity;
}