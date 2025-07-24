using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderService.Application.Abstractions;
using OrderService.Infrastructure.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.Infrastructure.Persistence.RabbitMQ;

public class RabbitMqPublisher<TRequest, TResponse> (
    IOptions<RabbitMqOptions> options
    ): IQueuePublisher<TRequest, TResponse>, IDisposable, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _replyChanelName;
    private AsyncEventingBasicConsumer? _consumer;
    
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pending 
        = new();
    private async Task InitializeConnection(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = options.Value.HostName,
            Port = options.Value.Port,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        

        var queueDeclare = await _channel.QueueDeclareAsync(cancellationToken: cancellationToken);
        _replyChanelName = queueDeclare.QueueName;
        

        _consumer = new AsyncEventingBasicConsumer(
            _channel
        );
        _consumer.ReceivedAsync += (model, ea) =>
        {
            string? correlationId = ea.BasicProperties.CorrelationId;
            
            if(!string.IsNullOrEmpty(correlationId))
            {
                if (_pending.TryRemove(correlationId, out var tcs))
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    tcs.TrySetResult(json);
                }
            }

            return Task.CompletedTask;
        };
        
        await _channel.BasicConsumeAsync(_replyChanelName, true, _consumer, cancellationToken: cancellationToken);
    }

    public async Task<TResponse?> PublishAsync(TRequest message, CancellationToken cancellationToken = default)
    {
        if (_connection == null || _channel == null)
        {
            await InitializeConnection(cancellationToken);
        }
        
        var correlationId = Guid.NewGuid().ToString();
        var properties = new BasicProperties()
        {
            CorrelationId = correlationId,
            ReplyTo = _replyChanelName
        };
        
        
        var tcs = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        _pending.TryAdd(correlationId, tcs);
        
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: options.Value.QueueName,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken
        );
        
        var response = await tcs.Task;
        var responseMessage = JsonSerializer.Deserialize<TResponse>(response);
        
        return responseMessage;
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        if (_channel != null) await _channel.DisposeAsync();
    }
}