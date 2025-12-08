namespace Ordering.Application.Extensions;

public class OrderNotFoundException(string name, Object key) : ApplicationException($"Entity {name} with key {key} was not found");