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
    
    public async Task<Product?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);   
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken: cancellationToken);
        try
        {
            var productEntity = await _products.AddAsync(product, cancellationToken: cancellationToken);
            await transaction.CommitAsync(cancellationToken: cancellationToken);
            return productEntity.Entity;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken: cancellationToken);
            throw;
        }
    }
    
    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken: cancellationToken);
        try
        {
            _products.Update(product);
            await transaction.CommitAsync(cancellationToken: cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken: cancellationToken);
            throw;
        }
    }
}