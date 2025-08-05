using OrderService.Application.Abstractions;

namespace OrderService.Application.Handlers;

public interface IUnitOfWork
{
    public IOrderRepository OrderRepository { get; }
    
    public IIdempotencyRepository IdempotencyKeyRepository { get; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);  
}