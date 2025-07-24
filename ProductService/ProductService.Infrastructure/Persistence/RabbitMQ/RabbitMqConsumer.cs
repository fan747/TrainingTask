using System.ComponentModel;
using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductService.Application.Abstractions;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Infrastructure.Options;
using ProductService.Infrastructure.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProductService.Infrastructure.Persistence.RabbitMQ;

public class RabbitMqConsumer<TRequest>(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<RabbitMqOptions> options
    ) : BackgroundService, IQueueConsumer, IDisposable where TRequest : CheckProductDto
{
    private IConnection? _connection;
    private IChannel? _channel;
    
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
        

        await _channel.QueueDeclareAsync(
            queue: "check_products_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );
        
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeConnection(stoppingToken);
        
        if (_channel != null)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var cons = (AsyncEventingBasicConsumer)sender;
                var channel = cons.Channel;
                var response = string.Empty;
                
                var body = ea.Body.ToArray();
                
                var props = new BasicProperties()
                {
                    CorrelationId = ea.BasicProperties.CorrelationId
                };
                
                var message = Encoding.UTF8.GetString(body);

                TRequest? requestDto;

                try
                {
                    requestDto = JsonSerializer.Deserialize<TRequest>(message);
                }
                catch (Exception e)
                {
                    requestDto = new CheckProductDto(-1, 0) as TRequest;
                }
                
                
                using var scope = serviceScopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var checkAndReserveCommand = new CheckAndReserveCommand(requestDto);
                var checkResult = await mediator.Send(checkAndReserveCommand, stoppingToken);
                response = JsonSerializer.Serialize(checkResult);
     
                
                var responseBody = Encoding.UTF8.GetBytes(response);
                await _channel.BasicPublishAsync(
                    exchange: string.Empty, 
                    routingKey: ea.BasicProperties.ReplyTo, 
                    mandatory: true, 
                    basicProperties: props, 
                    body: responseBody, 
                    cancellationToken: stoppingToken);
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            };
            await _channel.BasicConsumeAsync(options.Value.QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        }
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