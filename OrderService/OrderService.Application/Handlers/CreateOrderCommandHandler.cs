using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Results;

namespace OrderService.Application.Handlers;

public class CreateOrderCommandHandler(
    IUnitOfWork repository,
    IQueuePublisher<CheckProductDto, Result<CheckProductDto>> queuePublisher   
    ) : IRequestHandler<CreateOrderCommand, Result>
{
    public async Task<Result> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await queuePublisher.PublishAsync(request.CheckProductDto, cancellationToken);

            if (result == null)
            {
                throw new Exception("Product not found");
            }

            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorMessage);
            }
            
            await repository.OrderRepository.AddAsync(Order.Create(request.CheckProductDto.ProductId, request.CheckProductDto.Quantity), cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);       
        }
    }
}