using ProductService.Domain.Entities;

namespace ProductService.Application.Abstractions;

public interface IProductRepository
{
    Task<Product?> GetById(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}