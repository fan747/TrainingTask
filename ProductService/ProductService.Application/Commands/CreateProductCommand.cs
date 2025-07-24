using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Results;

namespace ProductService.Application.Commands;

public record CreateProductCommand(CreateProductDto CreateProductDto) : IRequest<Result>;
