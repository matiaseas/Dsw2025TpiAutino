namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public Guid CustomerId { get; private set; }
        public Customer? Customer { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal Total { get; private set; }
        public OrderStatus Status { get; private set; }

        // Detalles de la orden
        public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

        public Order() { }

        public Order(Guid customerId, DateTime createdAt)
        {
            CustomerId = customerId;
            CreatedAt = createdAt;
            Status = OrderStatus.Pending;
        }

        /// <summary>
        /// Agrega un item a la orden.
        /// </summary>
        public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
        {
            var lineTotal = unitPrice * quantity;
            var item = new OrderItem(this.Id, productId, productName, unitPrice, quantity, lineTotal);
            Items.Add(item);
        }

        /// <summary>
        /// Establece el subtotal de la orden.
        /// </summary>
        public void SetSubtotal(decimal subtotal)
        {
            Subtotal = subtotal;
        }

        /// <summary>
        /// Establece el total de la orden.
        /// </summary>
        public void SetTotal(decimal total)
        {
            Total = total;
        }

        /// <summary>
        /// Actualiza el estado de la orden.
        /// </summary>
        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }
    }
}
