using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;
using ProductService.Application.Results;

namespace ProductService.API.Controllers;

[ApiController]
[Route("products")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> Get([FromRoute] int id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));

        return result.ErrorType switch
        {
            ErrorType.BadRequest => BadRequest(result.ErrorMessage),
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.InternalServerError => StatusCode(500, result.ErrorMessage),
            null => Ok(result.Data),
            _ => StatusCode(500, result.ErrorMessage)
        };  
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Add([FromBody] CreateProductDto createProductDto)
    {
        var result = await mediator.Send(new CreateProductCommand(createProductDto));

        return result.ErrorType switch
        {
            ErrorType.BadRequest => BadRequest(result.ErrorMessage),
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.InternalServerError => StatusCode(500, result.ErrorMessage),
            null => Ok(result.Data),
            _ => StatusCode(500, result.ErrorMessage)
        };  
    }
    
    [HttpPut("{id:int}/stock")]
    public async Task<ActionResult> PutQuantity([FromRoute] int id, [FromBody] PutQuantityProductDto quantity)
    {
        var updateCommand = new UpdateProductCommand(new ProductDto(id, Quantity: quantity.Quantity));
        
        var result = await mediator.Send(updateCommand);
        
        return result.ErrorType switch
        {
            ErrorType.BadRequest => BadRequest(result.ErrorMessage),
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.InternalServerError => StatusCode(500, result.ErrorMessage),
            null => NoContent(),
            _ => StatusCode(500, result.ErrorMessage)
        };  
    }
}