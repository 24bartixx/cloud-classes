using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared;
using Shared;

namespace ConsumerApp.Consumers;

public class TransferConsumer(int id, RabbitBrokerClient brokerClient) : BaseConsumer<Type3Event>(id, brokerClient), IPublisher
{

    protected override async Task OnEventReceived(Type3Event receivedEvent)
    {
        LoggerHelper.LogCall($"Consumer {Id} received {nameof(Type3Event)} to {QueueName} with {receivedEvent.Message} at {receivedEvent.Timestamp}");

        var type4Event = new Type4Event 
        { 
            Message = $"Generated from Type 3 ID: {receivedEvent.Id}" 
        };
        
        await PublishEventAsync(type4Event);
    }

    public Task PublishEventAsync()
    {
        throw new NotSupportedException();
    }

    public async Task PublishEventAsync(CustomEvent customEvent)
    {
        if (Channel == null) return;
        
        var targetQueue = BrokerReflection.GetQueueName<Type4Event>();
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(customEvent));

        await Channel.QueueDeclareAsync(queue: targetQueue, durable: false, exclusive: false, autoDelete: false);
        
        await Channel.BasicPublishAsync(
            exchange: string.Empty, 
            routingKey: targetQueue, 
            body: body);

        LoggerHelper.LogCall($"Publisher {Id} sent {nameof(Type4Event)} to {targetQueue}");
    }
}