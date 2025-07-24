using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Results;

namespace ProductService.Application.Commands;

public record CheckAndReserveCommand(CheckProductDto CheckProductDto) : IRequest<Result<CheckProductDto>>;
