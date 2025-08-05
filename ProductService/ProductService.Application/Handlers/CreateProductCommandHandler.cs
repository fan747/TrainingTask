using AutoMapper;
using MediatR;
using ProductService.Application.Abstractions;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Results;
using ProductService.Domain.Entities;

namespace ProductService.Application.Handlers;

public class CreateProductCommandHandler(
    IUnitOfWork repository,
    IMapper mapper   
    ) : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.CreateProductDto.Quantity <= 0)
            {
                return Result<ProductDto>.Failure(ErrorType.BadRequest, "Quantity must be positive");
            }
            
            if (request.CreateProductDto.Price <= 0)
            {
                return Result<ProductDto>.Failure(ErrorType.BadRequest, "Price must be positive");
            }

            var product = mapper.Map<Product>(request.CreateProductDto);
            var createdProduct = await repository.ProductRepository.AddAsync(product, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var createdProductDto = mapper.Map<ProductDto>(createdProduct);
            return Result<ProductDto>.Success(createdProductDto);       
        }
        catch (Exception e)
        {
            return Result<ProductDto>.Failure(ErrorType.InternalServerError, e.Message);       
        }
    }
}