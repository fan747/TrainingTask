using MediatR;
using ProductService.Application.Abstractions;
using ProductService.Application.Commands;
using ProductService.Application.Results;

namespace ProductService.Application.Handlers;

public class UpdateProductCommandHandler(
    IUnitOfWork repository   
    ) : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ProductDto.Quantity <= 0)
            {
                return Result.Failure(ErrorType.BadRequest, "Quantity must be greater than 0");    
            }

            if (request.ProductDto.Price <= 0)
            {
                return Result.Failure(ErrorType.BadRequest, "Price must be greater than 0");    
            }
            
            var product = await repository.ProductRepository.GetById(request.ProductDto.Id, cancellationToken);

            if (product == null)
            {
                return Result.Failure(ErrorType.NotFound, "Product not found");
            }
            
            product.Update(
                request.ProductDto.Name ?? product.Name, 
                request.ProductDto.Quantity ?? product.Quantity, 
                request.ProductDto.Price ?? product.Price
                );
            
            await repository.ProductRepository.UpdateAsync(product, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success(); 
            
        }
        catch (Exception e)
        {
            return Result.Failure(ErrorType.InternalServerError, e.Message);
        }
    }
}