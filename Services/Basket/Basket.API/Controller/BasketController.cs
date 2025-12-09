using Basket.Application.Mappers;
using Basket.Core.Entities;
using EventBus.Messages.Common;
using MassTransit;

namespace Basket.API.Controller;
[ApiVersion("1.0")]
public class BasketController(IMediator mediator,IPublishEndpoint endpoint) : ApiController
{
    IMediator _mediator { get; } = mediator;
    IPublishEndpoint _publishEndpoint { get; } = endpoint;

    [HttpGet]
    [Route("[action]/{userName}", Name = "GetBasketByUserName")]
    [ProducesResponseType(typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCartResponse>> GetBasket(string userName)
    {
        var query = new GetBasketByUserNameQuery(userName);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost("CreateBasket")]
    [ProducesResponseType(typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCartResponse>> UpdateBasket([FromBody] CreateShoppingCartCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete]
    [Route("[action]/{userName}", Name = "DeleteBasketByUserName")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
        var cmd = new DeleteBasketByUserNameCommand(userName);
        return Ok(await _mediator.Send(cmd));
    }
    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
        var basket = await _mediator.Send(query);
        if (basket == null)
        {
            return BadRequest();
        }
        var eventMsg = BasketMapper.Mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMsg.TotalPrice = basket.TotalPrice;
        await _publishEndpoint.Publish(eventMsg);
        var deleteCmd = new DeleteBasketByUserNameCommand(basket.UserName);
        await _mediator.Send(deleteCmd);
        return Accepted();
    }
    
}