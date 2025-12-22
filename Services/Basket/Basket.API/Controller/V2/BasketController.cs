using Basket.Application.Mappers;
using Basket.Core.Entities;
using EventBus.Messages.Common;
using MassTransit;

namespace Basket.API.Controller.V2;

[ApiVersion("2.0")]
public class BasketController(IMediator mediator, IPublishEndpoint endpoint, ILogger<BasketController> logger) : ApiController
{
    IMediator _mediator { get; } = mediator;
    IPublishEndpoint _publishEndpoint { get; } = endpoint;
    ILogger<BasketController> _logger { get; } = logger;
    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckoutV2 basketCheckout)
    {
        var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
        var basket = await _mediator.Send(query);
        if (basket == null)
        {
            return BadRequest();
        }
        var eventMsg = BasketMapper.Mapper.Map<BasketCheckoutEventV2>(basketCheckout);
        eventMsg.TotalPrice = basket.TotalPrice;
        await _publishEndpoint.Publish(eventMsg);
        _logger.LogInformation("Basket Published for {BasketUserName} with V2 endpoint", basket.UserName);
        var deleteCmd = new DeleteBasketByUserNameCommand(basket.UserName);
        await _mediator.Send(deleteCmd);
        return Accepted();
    }
}
