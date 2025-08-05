using Microsoft.EntityFrameworkCore;
using OrderService.Application.Abstractions;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories;

public class IdempotencyKeyRepository(OrderDbContext context) : IIdempotencyRepository
{
    
    private DbSet<IdempotencyKey> _idempotencyKeys = context.IdempotencyKeys;

    public async Task AddAsync(IdempotencyKey orderRequest, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var entity = await context.IdempotencyKeys.AddAsync(orderRequest, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IdempotencyKey?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _idempotencyKeys.FirstOrDefaultAsync(o=>o.Id == id, cancellationToken);
    }

    public async Task<IdempotencyKey?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _idempotencyKeys.FirstOrDefaultAsync(o=>o.Key == key, cancellationToken);
    }

    public async Task DeleteAsync(IdempotencyKey entity, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _idempotencyKeys.Remove(entity);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}