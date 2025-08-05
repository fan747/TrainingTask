using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Results;

namespace OrderService.Application.Commands;

public record CreateOrderCommand(
    string IdempotencyKey,
    CheckProductDto CheckProductDto
) : IRequest<Result<OrderDto>>;
