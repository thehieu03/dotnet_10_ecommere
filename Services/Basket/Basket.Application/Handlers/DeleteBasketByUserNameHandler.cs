namespace Basket.Application.Handlers;

public class DeleteBasketByUserNameHandler(IBasketRepository basketRepository)
    : IRequestHandler<DeleteBasketByUserNameCommand, Unit>
{
    private readonly IBasketRepository _basketRepository = basketRepository;

    public async Task<Unit> Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)
    {
        await _basketRepository.DeleteBasketAsync(request.UserName);
        return Unit.Value;
    }
}