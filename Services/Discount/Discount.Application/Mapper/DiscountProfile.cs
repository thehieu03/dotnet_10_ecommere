namespace Discount.Application.Mapper;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        CreateMap<Coupon,CouponModel>();
    }
}