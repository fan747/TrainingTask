using Microsoft.EntityFrameworkCore;
using ProductService.Application.Abstractions;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository(
    ProductDbContext context   
    ) : IProductRepository
{
    private readonly DbSet<Product> _products = context.Products;
    
    public async Task<Product?> GetById(int id)
    {
        return await _products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);   
    }

    public async Task AddAsync(Product product)
    {
        await _products.AddAsync(product);
    }
    
    public Task UpdateAsync(Product product)
    {
        _products.Update(product);
        return Task.CompletedTask;
    }
}