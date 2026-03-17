using Shared;

namespace ConsumerApp.Consumers;

public class SimpleConsumer<T>(int id, RabbitBrokerClient brokerClient) : BaseConsumer<T>(id, brokerClient) 
    where T : CustomEvent, new()
{
    protected override Task OnEventReceived(T receivedEvent)
    {
        LoggerHelper.LogCall($"Consumer {Id} received {typeof(T).Name} to {QueueName} with {receivedEvent.Message} at {receivedEvent.Timestamp}");
        return Task.CompletedTask;
    }
}