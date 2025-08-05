using Microsoft.EntityFrameworkCore;
using OrderService.Application.Handlers;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<IdempotencyKey> IdempotencyKeys { get; set; }
}
