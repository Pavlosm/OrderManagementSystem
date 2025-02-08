using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderManagementService.Core.Entities;

namespace OrderManagementService.Infrastructure;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<OrderContactDetails> OrderContactDetails { get; set; }
    
    public DbSet<OrderDeliveryAddress> OrderDeliveryAddresses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<MenuItem>()
            .HasKey(menuItem => menuItem.Id);
        
        modelBuilder
            .Entity<MenuItem>()
            .Property(menuItem => menuItem.Price)
            .HasColumnType("decimal(18,2)");
        
        modelBuilder
            .Entity<Order>()
            .HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(c => c.OrderId);
        
        modelBuilder
            .Entity<Order>()
            .Property(order => order.Status)
            .HasColumnType("tinyint");
        
        modelBuilder
            .Entity<Order>()
            .HasIndex(i => i.Status);
        
        modelBuilder
            .Entity<Order>()
            .Property(order => order.Type)
            .HasColumnType("tinyint");
        
        modelBuilder
            .Entity<Order>()
            .HasIndex(i => i.Type);
        
        modelBuilder
            .Entity<Order>()
            .HasOne<OrderDeliveryAddress>()
            .WithOne()
            .HasForeignKey<OrderDeliveryAddress>(a => a.OrderId);
        
        modelBuilder
            .Entity<Order>()
            .HasOne<OrderContactDetails>()
            .WithOne()
            .HasForeignKey<OrderContactDetails>(a => a.OrderId);
        
        modelBuilder
            .Entity<Order>()
            .Property(order => order.TotalAmount)
            .HasColumnType("decimal(18,2)");
        
        modelBuilder
            .Entity<OrderItem>()
            .Property(orderItem => orderItem.UnitPrice)
            .HasColumnType("decimal(18,2)");
        
        modelBuilder
            .Entity<OrderItem>()
            .HasOne<MenuItem>()
            .WithMany()
            .HasForeignKey(i => i.MenuItemId);
        
        base.OnModelCreating(modelBuilder);
    }
}