namespace OrderService.Application.Handlers;

public interface IUnitOfWork
{
    public IOrderRepository OrderRepository { get; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);  
}