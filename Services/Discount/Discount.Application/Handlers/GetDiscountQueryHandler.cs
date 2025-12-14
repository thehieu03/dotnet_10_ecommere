using Microsoft.Extensions.Logging;

namespace Discount.Application.Handlers;

public class GetDiscountQueryHandler(IDiscountRepository repository,ILogger<GetDiscountQueryHandler> logger) : IRequestHandler<GetDiscountQuery, CouponModel>
{
    public async Task<CouponModel> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
    {
        var coupon = await repository.GetDiscountAsync(request.ProductName);
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
        logger.LogInformation($"Copon for the {request.ProductName} is fetched");
        return couponModel;
    }
}