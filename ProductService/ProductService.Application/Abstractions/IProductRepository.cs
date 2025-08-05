using ProductService.Domain.Entities;

namespace ProductService.Application.Abstractions;

public interface IProductRepository
{
    Task<Product?> GetById(int id, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}