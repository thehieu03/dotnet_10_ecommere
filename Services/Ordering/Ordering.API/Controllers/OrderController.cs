using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;

namespace Ordering.API.Controllers;

[Authorize(Policy = "Public")] // Public by default
public class OrderController : ApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderController> _logger;
    public OrderController(IMediator mediator, ILogger<OrderController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    [HttpGet("{userName}", Name = "GetOrdersByUsername")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), (int)StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAllOrders(string userName)
    {
        var query = new GetOrderListQuery(userName);
        var result = await _mediator.Send(query);
        // var result=await _mediator.Send()
        return Ok(result);
    }
    // Just for testing
    [HttpPost(Name = "CheckoutOrder")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderResponse>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpPut(Name = "UpdateOrder")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return NoContent();
    }
    [HttpDelete("{id}", Name = "DeleteOrder")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        var cmd = new DeleteOrderCommand
        {
            OrderId = id
        };
        await _mediator.Send(cmd);
        return NoContent();
    }


}