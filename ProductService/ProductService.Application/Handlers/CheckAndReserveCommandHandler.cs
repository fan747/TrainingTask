using MediatR;
using ProductService.Application.Abstractions;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Results;

namespace ProductService.Application.Handlers;

public class CheckAndReserveCommandHandler(
    IUnitOfWork unitOfWork
    ) : IRequestHandler<CheckAndReserveCommand, Result<CheckProductDto>>
{
    public async Task<Result<CheckProductDto>> Handle(CheckAndReserveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await unitOfWork.ProductRepository.GetById(request.CheckProductDto.ProductId);

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            if (product.Quantity < request.CheckProductDto.Quantity)
            {
                throw new Exception("Insufficient quantity of product");
            }
            
            product.Update(
                product.Name,
                product.Quantity - request.CheckProductDto.Quantity,
                product.Price
            );

            await unitOfWork.ProductRepository.UpdateAsync(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<CheckProductDto>.Success(request.CheckProductDto);       
        }
        catch (Exception e)
        {
            return Result<CheckProductDto>.Failure(e.Message);
        }
    }
}