

namespace Basket.Application.GrpcService;
public class DiscountGrpcService(DiscountService.DiscountServiceClient discountClient)
{
    public async Task<CouponModel> GetDiscount(string productName)
    {
        var discountRequest = new GetDiscountRequest() { ProductName = productName };
        return await discountClient.GetDiscountAsync(discountRequest);
    }
}
