namespace Discount.Application.Handlers;

public class CreateDiscountCommandHandler(IDiscountRepository repository, IMapper mapper)
    : IRequestHandler<CreateDiscountCommand, CouponModel>
{
    public async Task<CouponModel> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        var coupon = mapper.Map<Coupon>(request);
        await  repository.CreateDiscountAsync(coupon);
        var couponModel = mapper.Map<CouponModel>(coupon);
        return couponModel;
    }
}