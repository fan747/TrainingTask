using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Application.Results;

namespace OrderService.Web.Controllers;

[ApiController]
[Route("orders")]
public class OrderController(
    IMediator mediator   
    ) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Add([FromBody] CreateOrderDto createOrder)
    {
        var command = new CreateOrderCommand(createOrder.IdempotencyKey, createOrder.CheckProduct);
        var result = await mediator.Send(command);

        return result.ErrorType switch
        {
            ErrorType.BadRequest => BadRequest(result.ErrorMessage),
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.InternalServerError => StatusCode(500, result.ErrorMessage),
            null => Ok(result.Data),
            _ => StatusCode(500, result.ErrorMessage)
        };
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> Get([FromRoute] int id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await mediator.Send(query);
        
        return result.ErrorType switch
        {
            ErrorType.BadRequest => BadRequest(result.ErrorMessage),
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.InternalServerError => StatusCode(500, result.ErrorMessage),
            null => Ok(result.Data),
            _ => StatusCode(500, result.ErrorMessage)
        };      
    }
}