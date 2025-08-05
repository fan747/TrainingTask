using OrderService.Domain.Entities;

namespace OrderService.Application.Abstractions;

public interface IIdempotencyRepository
{
    Task AddAsync(IdempotencyKey orderRequest, CancellationToken cancellationToken = default);
    Task<IdempotencyKey?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IdempotencyKey?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(IdempotencyKey entity, CancellationToken cancellationToken = default);
}