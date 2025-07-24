using AutoMapper;
using MediatR;
using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;
using ProductService.Application.Results;
using ProductService.Domain.Entities;

namespace ProductService.Application.Handlers;

public class GetProductByIdQueryHandler(
    IUnitOfWork repository,
    IMapper mapper
    ) : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await repository.ProductRepository.GetById(request.Id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }
            
            var productDto = mapper.Map<ProductDto>(product);
            return Result<ProductDto>.Success(productDto);
        }
        catch (Exception e)
        {
            return Result<ProductDto>.Failure(e.Message);
        }
    }
}