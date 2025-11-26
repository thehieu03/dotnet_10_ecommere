namespace Basket.Application.Commands;

public abstract class DeleteBasketByUserNameCommand(string userName) : IRequest<Unit>
{
    public string UserName { get; set; } = userName;

}