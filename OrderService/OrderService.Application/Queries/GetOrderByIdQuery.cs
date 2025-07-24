using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Results;

namespace OrderService.Application.Queries;

public record GetOrderByIdQuery(int Id) : IRequest<Result<OrderDto>>;
