namespace Shared;

public static class BrokerReflection
{
    public static string GetQueueName<T>()
    {
        var type = typeof(T);
        
        var name = type.Name;
        
        return name.EndsWith("Event") 
            ? name 
            : throw new ArgumentException($"Class {name} does not end with 'Event'!");
    }
}