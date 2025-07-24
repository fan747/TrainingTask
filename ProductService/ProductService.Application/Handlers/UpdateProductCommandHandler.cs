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
            var product = await repository.ProductRepository.GetById(request.ProductDto.Id);

            if (product == null)
            {
                throw new Exception("Product not found"); 
            }
            
            product.Update(
                request.ProductDto.Name ?? product.Name, 
                request.ProductDto.Quantity ?? product.Quantity, 
                request.ProductDto.Price ?? product.Price
                );
            
            await repository.ProductRepository.UpdateAsync(product);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success(); 
            
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }
}