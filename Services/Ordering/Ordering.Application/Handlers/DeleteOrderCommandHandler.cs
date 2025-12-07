using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Application.Extensions;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Handlers;

public class DeleteOrderCommandHandler(IOrderRepository orderRepository, ILogger logger) : IRequestHandler<DeleteOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ILogger _logger = logger;
    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var orderToDelete = await _orderRepository.GetByIdAsync(request.OrderId);
        if (orderToDelete == null)
        {
            throw new OrderNotFoundException(nameof(Order), request.OrderId);
        }
        await _orderRepository.DeleteAsync(orderToDelete);
        return Unit.Value;
    }
}