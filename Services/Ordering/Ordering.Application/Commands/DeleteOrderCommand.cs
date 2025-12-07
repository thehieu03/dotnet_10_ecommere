using MediatR;

namespace Ordering.Application.Commands;

public class DeleteOrderCommand:IRequest<Unit>
{
    public int OrderId { get; set; }
}