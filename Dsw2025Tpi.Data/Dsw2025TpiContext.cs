using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext : DbContext
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Productos
        var dbProduct = modelBuilder.Entity<Product>().ToTable("Products");
        dbProduct.HasKey(p => p.Id);
        dbProduct.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);
        dbProduct.HasIndex(p => p.Sku).IsUnique();
        dbProduct.Property(p => p.InternalCode)
            .IsRequired()
            .HasMaxLength(50);
        dbProduct.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        dbProduct.Property(p => p.Description)
            .HasMaxLength(500);
        dbProduct.Property(p => p.CurrentUnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();
        dbProduct.Property(p => p.StockQuantity)
            .IsRequired();
        dbProduct.Property(p => p.IsActive)
            .HasDefaultValue(true);

        // Clientes
        var dbCustomer = modelBuilder.Entity<Customer>().ToTable("Customers");
        dbCustomer.HasKey(c => c.Id);
        dbCustomer.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        dbCustomer.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        // Órdenes
        var dbOrder = modelBuilder.Entity<Order>().ToTable("Orders");
        dbOrder.HasKey(o => o.Id);
        dbOrder.Property(o => o.CreatedAt)
            .IsRequired();
        dbOrder.Property(o => o.Subtotal)
            .HasPrecision(18, 2);
        dbOrder.Property(o => o.Total)
            .HasPrecision(18, 2);
        dbOrder.Property(o => o.Status)
            .IsRequired();
        dbOrder.HasOne(o => o.Customer)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        // Detalles de Orden
        var dbOrderItem = modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
        dbOrderItem.HasKey(oi => oi.Id);
        dbOrderItem.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(100);
        dbOrderItem.Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();
        dbOrderItem.Property(oi => oi.Quantity)
            .IsRequired();
        dbOrderItem.Property(oi => oi.LineTotal)
            .HasPrecision(18, 2)
            .IsRequired();
        dbOrderItem.HasOne(oi => oi.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
    }
}
