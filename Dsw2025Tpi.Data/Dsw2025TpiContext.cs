using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext: DbContext
{
	public Dsw2025TpiContext(DbContextOptions options) : base(options) { }

	public DbSet<Product> Products { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		var dbProduct = modelBuilder.Entity<Product>().ToTable("Products");
		dbProduct.Property(p => p.Sku)
			.IsRequired()
			.HasMaxLength(50);
		dbProduct.HasIndex(p => p.Sku)
			.IsUnique();
		dbProduct.Property(p => p.InternalCode)
			.IsRequired()
			.HasMaxLength(50);
		dbProduct.Property(p => p.Name)
			.HasMaxLength(50)
			.IsRequired();
		dbProduct.Property(p => p.Description)
			.HasMaxLength(300);
		dbProduct.Property(p => p.CurrentUnitPrice)
			.HasPrecision(15, 2)
			.IsRequired();
		dbProduct.Property(p => p.StockQuantity)
			.IsRequired();
		/*
		var dbCustomer = modelBuilder.Entity<Customer>().ToTable("Customers");
		dbCustomer.Property(c => c.Name)
			.HasMaxLength(100)
			.IsRequired();
		dbCustomer.Property(c => c.Email)
			.HasMaxLength(100)
			.IsRequired();
		dbCustomer.Property(c => c.PhoneNumber)
			.HasMaxLength(15)
			.IsRequired();
		var dbOrder = modelBuilder.Entity<Order>().ToTable("Orders");
		dbOrder.Property(o => o.CreateDate)
			.IsRequired();
		dbOrder.Property(o => o.ShippingAddress)
			.HasMaxLength(200)
			.IsRequired();
		dbOrder.Property(o => o.BillingAddress)
			.HasMaxLength(200)
			.IsRequired();
		dbOrder.Property(o => o.Notes)
			.HasMaxLength(500);
		dbOrder.Property(o => o.TotalAmount)
			.HasPrecision(15, 2)
			.IsRequired();
		var dbOrderItem = modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
		dbOrderItem.Property(oi => oi.Quantity)
			.IsRequired();
		dbOrderItem.Property(oi => oi.UnitPrice)
			.HasPrecision(15, 2)
			.IsRequired();
		dbOrderItem.Property(oi => oi.Subtotal)
			.HasPrecision(15, 2)
			.IsRequired();
		modelBuilder.Entity<Order>()
			.HasOne(o => o.Customer)
			.WithMany(c => c.Orders)
			.HasForeignKey(o => o.CustomerId);

		modelBuilder.Entity<OrderItem>()
			.HasOne(oi => oi.Order)
			.WithMany(o => o.OrderItems)
			.HasForeignKey(oi => oi.OrderId);

		modelBuilder.Entity<OrderItem>()
			.HasOne(oi => oi.Product)
			.WithMany(p => p.OrderItems)
			.HasForeignKey(oi => oi.ProductId);
	}
	*/
	/*
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Category>(eb =>
		{
			eb.ToTable("Categories");
			eb.Property(p => p.Name)
			.HasMaxLength(50)
			.IsRequired();
		});
		modelBuilder.Entity<Product>(eb =>
		{
			eb.ToTable("Products");
			eb.Property(p => p.Sku)
			.HasMaxLength(20)
			.IsRequired();
			eb.Property(p => p.Name)
			.HasMaxLength(60);
			eb.Property(p => p.CurrentUnitPrice)
			.HasPrecision(15, 2);
		});
	*/
}
}
