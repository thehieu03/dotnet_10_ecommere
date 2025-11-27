namespace Discount.Application.Handlers;

public class DeleteDiscountCommandHandler(IDiscountRepository discountRepository)
    : IRequestHandler<DeleteDiscountCommand, bool>
{
    private readonly IDiscountRepository _discountRepository = discountRepository;

    public async Task<bool> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _discountRepository.DeleteDiscountAsync(request.ProductName);
        return deleted;
    }
}