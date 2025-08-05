using Microsoft.EntityFrameworkCore;
using OrderService.Application.Handlers;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository(
    OrderDbContext context   
    ) : IOrderRepository
{
    private DbSet<Order> _orders = context.Orders;
    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var orderEntity = await _orders.AddAsync(order, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return orderEntity.Entity;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Order?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _orders.AsNoTracking().FirstOrDefaultAsync(o=>o.Id == id, cancellationToken);
    }
}