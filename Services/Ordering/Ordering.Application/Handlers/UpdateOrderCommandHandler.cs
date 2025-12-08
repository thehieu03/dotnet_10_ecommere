using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Application.Extensions;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Handlers;

public class UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<UpdateOrderCommandHandler> logger)
    : IRequestHandler<UpdateOrderCommand, Unit>
{

    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetByIdAsync(request.Id);
        if (orderToUpdate == null)
        {
            throw new OrderNotFoundException(nameof(Order), request.Id);
        }
        mapper.Map(request,orderToUpdate,typeof(UpdateOrderCommand),typeof(Order));
        await orderRepository.UpdateAsync(orderToUpdate);
        logger.LogInformation($"Order {orderToUpdate.Id} updated");
        return Unit.Value;
    }
}