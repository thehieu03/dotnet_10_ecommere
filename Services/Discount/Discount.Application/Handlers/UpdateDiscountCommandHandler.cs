namespace Discount.Application.Handlers;

public class UpdateDiscountCommandHandler(IMapper mapper, IDiscountRepository repository)
    : IRequestHandler<UpdateDiscountCommand, CouponModel>
{
    private readonly  IMapper _mapper = mapper;
    private readonly IDiscountRepository _repository = repository;

    public async Task<CouponModel> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        var coupon = _mapper.Map<Coupon>(request);
        await _repository.UpdateDiscountAsync(coupon);
        var couponModel = _mapper.Map<CouponModel>(coupon);
        return couponModel;
    }
}