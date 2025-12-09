namespace EventBus.Messages.Common;

public class BaseIntegrationEvent
{
    public string CorelationId { get; set; }
    public DateTime CreationDate { get; set; }
    public BaseIntegrationEvent()
    {
        CorelationId=Guid.NewGuid().ToString();
        CreationDate = DateTime.UtcNow;
    }
    public BaseIntegrationEvent(Guid correlationId,DateTime creationDate)
    {
        CorelationId = correlationId.ToString();
        CreationDate = creationDate;
    }
}