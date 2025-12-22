namespace EventBus.Messages.Common;

public class BasketCheckoutEventV2 : BaseIntegrationEvent
{
    public string? UserName { get; set; }
    public decimal? TotalPrice { get; set; }
}