using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;

namespace OrderService.Web.Controllers;

[ApiController]
[Route("orders")]
public class OrderController(
    IMediator mediator   
    ) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> Add([FromBody] CheckProductDto checkProduct)
    {
        var command = new CreateOrderCommand(checkProduct);
        var result = await mediator.Send(command);
        
        return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.ErrorMessage);       
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> Get([FromRoute] int id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await mediator.Send(query);
        
        return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.ErrorMessage);      
    }
}