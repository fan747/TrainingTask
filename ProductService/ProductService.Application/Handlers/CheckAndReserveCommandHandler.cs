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
            var product = await unitOfWork.ProductRepository.GetById(request.CheckProductDto.ProductId, cancellationToken);

            if (product == null)
            {
                return Result<CheckProductDto>.Failure(ErrorType.NotFound, "Product not found");
            }

            if (product.Quantity < request.CheckProductDto.Quantity)
            {
                return Result<CheckProductDto>.Failure(ErrorType.BadRequest, "Not enough quantity");
            }
            
            product.Update(
                product.Name,
                product.Quantity - request.CheckProductDto.Quantity,
                product.Price
            );

            await unitOfWork.ProductRepository.UpdateAsync(product, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<CheckProductDto>.Success(request.CheckProductDto);       
        }
        catch (Exception e)
        {
            return Result<CheckProductDto>.Failure(ErrorType.InternalServerError, e.Message);
        }
    }
}