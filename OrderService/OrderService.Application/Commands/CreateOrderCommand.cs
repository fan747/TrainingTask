using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Results;

namespace OrderService.Application.Commands;

public record CreateOrderCommand(
    CheckProductDto CheckProductDto
) : IRequest<Result>;
