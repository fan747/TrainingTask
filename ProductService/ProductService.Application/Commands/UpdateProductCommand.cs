using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Results;

namespace ProductService.Application.Commands;

public record UpdateProductCommand(
    ProductDto ProductDto   
    ) : IRequest<Result>;
