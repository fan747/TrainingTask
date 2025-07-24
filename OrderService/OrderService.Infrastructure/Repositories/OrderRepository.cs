using Microsoft.EntityFrameworkCore;
using OrderService.Application.Handlers;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository(
    OrderDbContext context   
    ) : IOrderRepository
{
    private DbSet<Order> _orders = context.Orders;
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _orders.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _orders.AsNoTracking().FirstOrDefaultAsync(o=>o.Id == id, cancellationToken);
    }
}