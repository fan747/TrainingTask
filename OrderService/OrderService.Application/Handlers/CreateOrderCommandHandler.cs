using AutoMapper;
using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Results;
using OrderService.Domain.Entities;

namespace OrderService.Application.Handlers;

public class CreateOrderCommandHandler(
    IUnitOfWork repository,
    IQueuePublisher<CheckProductDto, Result<CheckProductDto>> queuePublisher ,
    IMapper mapper
    ) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if(request.CheckProductDto.Quantity <= 0)
            {
                return Result<OrderDto>.Failure(ErrorType.BadRequest, "Quantity must be greater than 0");
            }
            
            if(request.CheckProductDto.ProductId <= 0)
            {
                return Result<OrderDto>.Failure(ErrorType.BadRequest, "Product id must be greater than 0");
            }
            
            var idempotencyKey =
                await repository.IdempotencyKeyRepository.GetByKeyAsync(request.IdempotencyKey, cancellationToken);

            if (idempotencyKey != null)
            {
                return Result<OrderDto>.Failure(ErrorType.BadRequest, "Idempotency key already exists");
            }

            var newIdempotencyKey = IdempotencyKey.Create(request.IdempotencyKey);
            await repository.IdempotencyKeyRepository.AddAsync(newIdempotencyKey, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var result = await queuePublisher.PublishAsync(request.CheckProductDto, cancellationToken);

            if (result == null)
            {
                return Result<OrderDto>.Failure(ErrorType.InternalServerError, "Failed to publish message");
            }

            if (!result.IsSuccess || result.Data == null)
            {
                return Result<OrderDto>.Failure(ErrorType.BadRequest, "The product is out of stock");
            }
            
            if(result.Data.Quantity <= 0)
            {
                return Result<OrderDto>.Failure(ErrorType.BadRequest, "Quantity must be greater than 0");
            }
            
            if(result.Data.ProductId <= 0)
            {
                return Result<OrderDto>.Failure(ErrorType.BadRequest, "Product id must be greater than 0");
            }

            var order =await repository.OrderRepository.AddAsync(
                Order.Create(request.CheckProductDto.ProductId, request.CheckProductDto.Quantity), cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            
            var orderDto = mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Success(orderDto);
        }
        catch (Exception e)
        {
            return Result<OrderDto>.Failure(ErrorType.InternalServerError, e.Message);
        }
    }
}