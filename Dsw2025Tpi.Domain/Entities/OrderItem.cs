namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public Guid OrderId { get; private set; }
        public Order? Order { get; private set; }

        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal LineTotal { get; private set; }

        public OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, string productName, decimal unitPrice, int quantity, decimal lineTotal)
        {
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            LineTotal = lineTotal;
        }
    }
}
