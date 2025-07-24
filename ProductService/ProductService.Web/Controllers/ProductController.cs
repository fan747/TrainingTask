using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;

namespace ProductService.API.Controllers;

[ApiController]
[Route("products")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IResult> Get([FromRoute] int id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));

        return !result.IsSuccess ? Results.NotFound(result.ErrorMessage) : Results.Ok(result.Data);
    }

    [HttpPost]
    public async Task<IResult> Add([FromBody] CreateProductDto createProductDto)
    {
        var result = await mediator.Send(new CreateProductCommand(createProductDto));

        return !result.IsSuccess ? Results.BadRequest(result.ErrorMessage) : Results.NoContent();
    }
    
    [HttpPut("{id:int}/stock")]
    public async Task<IResult> PutQuantity([FromRoute] int id, [FromBody] PutQuantityProductDto quantity)
    {
        var updateCommand = new UpdateProductCommand(new ProductDto(id, Quantity: quantity.Quantity));
        
        var result = await mediator.Send(updateCommand);
        
        return !result.IsSuccess ? Results.BadRequest(result.ErrorMessage) : Results.NoContent();       
    }
}