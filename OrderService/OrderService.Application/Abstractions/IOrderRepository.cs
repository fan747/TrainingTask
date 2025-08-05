using OrderService.Domain.Entities;

namespace OrderService.Application.Handlers;

public interface IOrderRepository
{
    public Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
    public Task<Order?> GetById(int id, CancellationToken cancellationToken = default);
}