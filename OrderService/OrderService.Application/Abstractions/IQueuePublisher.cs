namespace OrderService.Application.Abstractions;

public interface IQueuePublisher<TRequest, TResponse>
{
    Task<TResponse?> PublishAsync(TRequest message, CancellationToken cancellationToken);
}