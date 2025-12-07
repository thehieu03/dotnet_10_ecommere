namespace Ordering.Application.Extensions;

public class OrderNotFoundException:ApplicationException
{
    public OrderNotFoundException(string name, Object key):base($"Entity {name} with key {key} was not found")
    {
        
    }
}