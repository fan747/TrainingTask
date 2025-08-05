using ProductService.Application.Abstractions;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Repositories;

public class UnitOfWork(
    ProductDbContext context
) : IUnitOfWork, IDisposable, IAsyncDisposable
{
    private IProductRepository _productRepository;

    public IProductRepository ProductRepository
    {
        get{ return _productRepository ??= new ProductRepository(context); }
    } 
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}