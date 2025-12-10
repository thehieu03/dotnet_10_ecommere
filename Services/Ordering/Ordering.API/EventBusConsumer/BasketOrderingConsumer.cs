using AutoMapper;
using EventBus.Messages.Common;
using MassTransit;
using MediatR;
using Ordering.Application.Commands;

namespace Ordering.API.EventBusConsumer;

public class BasketOrderingConsumer(IMediator mediator, IMapper mapper, ILogger<BasketOrderingConsumer> logger)
    : IConsumer<BasketCheckoutEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        using var scope = logger.BeginScope("Consuming Basket Checkout Event for {correlationId}",context.Message.CorelationId);
        var cmd=mapper.Map<CheckoutOrderCommand>(context.Message);
        var result = await mediator.Send(cmd);
        logger.LogInformation("Basket Checkout Event completed!!!");
    }
}