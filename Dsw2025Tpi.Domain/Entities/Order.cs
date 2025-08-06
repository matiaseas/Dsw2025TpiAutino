using Dsw2025Tpi.Domain.Entities;

public class Order : EntityBase
{
    public DateTime OrderDate { get; set; }
    public string ShippingAddress { get; set; }
    public string BillingAddress { get; set; }
    public string Notes { get; set; }
    public decimal TotalAmount { get; private set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public OrderStatus Status { get; set; }

    public Order() { }

    public Order(string shippingAddress, string billingAddress, string? notes, List<OrderItem> orderItems, Guid customerId)
    {
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Notes = notes;
        OrderDate = DateTime.UtcNow;
        OrderItems = orderItems ?? new List<OrderItem>();
        CustomerId = customerId;
        TotalAmount = CalculateTotalAmount();
        Status = OrderStatus.Pending;
    }

    public decimal CalculateTotalAmount() => OrderItems.Sum(item => item.SubTotal);
}