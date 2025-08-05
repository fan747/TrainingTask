using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Application.Results;

namespace OrderService.Application.Handlers;

public class GetOrderByIdQueryHandler(
    IUnitOfWork repository,
    IMapper mapper
    ) : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await repository.OrderRepository.GetById(request.Id, cancellationToken);
            
            if (order == null)
            {
                return Result<OrderDto>.Failure(ErrorType.NotFound,"Order not found");
            }
            
            var orderDto = mapper.Map<OrderDto>(order);
            
            return Result<OrderDto>.Success(orderDto);
        }
        catch (Exception e)
        {
            return Result<OrderDto>.Failure( ErrorType.InternalServerError,e.Message);
        }
    }
}