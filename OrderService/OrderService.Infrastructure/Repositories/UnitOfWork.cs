using OrderService.Application.Abstractions;
using OrderService.Application.Handlers;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories;

public class UnitOfWork(
    OrderDbContext context   
    ) : IUnitOfWork
{
    private IOrderRepository _orderRepository;

    public IOrderRepository OrderRepository
    {
        get { return _orderRepository ??= new OrderRepository(context); }
    }

    public IIdempotencyRepository IdempotencyKeyRepository => new IdempotencyKeyRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken);
    
}