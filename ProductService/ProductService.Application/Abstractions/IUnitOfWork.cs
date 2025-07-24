

namespace ProductService.Application.Abstractions;

public interface IUnitOfWork
{
    public IProductRepository ProductRepository { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);   
}