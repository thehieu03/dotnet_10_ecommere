using Discount.Core.Repositories;
using Grpc.Core;

namespace Discount.Application.Handlers;

public class GetDiscountQueryHandler(IDiscountRepository repository) : IRequestHandler<GetDiscountQuery, CouponModel>
{
    private readonly IDiscountRepository _repository = repository;

    public async Task<CouponModel> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
    {
        var coupon = await _repository.GetDiscountAsync(request.ProductName);
        if (coupon == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Discount for the Product Name = {request.ProductName} not found."));
        }

        var couponModel = new CouponModel()
        {
            Id = coupon.Id,
            Amount = coupon.Amount,
            Description = coupon.Description,
            ProductName = coupon.ProductName
        };
        return couponModel;
    }
}