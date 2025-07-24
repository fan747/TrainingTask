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
    ) : IRequestHandler<CreateProductCommand, Result>
{
    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = mapper.Map<Product>(request.CreateProductDto);
            await repository.ProductRepository.AddAsync(product);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success();       
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);       
        }
    }
}