using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Results;

namespace ProductService.Application.Queries;

public record GetProductByIdQuery(int Id) : IRequest<Result<ProductDto>>;
