using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared;

namespace PublisherApp.Publishers;

public abstract class BasePublisher<T>(int id, RabbitBrokerClient brokerClient)
    where T : CustomEvent, new()
{
    protected readonly int Id = id;
    protected IChannel? Channel;
    protected string? QueueName;

    public virtual async Task RunAsync(CancellationToken ct = default)
    {
        Channel = await brokerClient.CreateChannelAsync();
        QueueName = BrokerReflection.GetQueueName<T>();

        await Channel.QueueDeclareAsync(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null, cancellationToken: ct);
            
        LoggerHelper.LogCall($"Publisher {Id} initialized for queue {QueueName}");
    }
    
    protected async Task PublishEventAsync()
    {
        if (Channel == null || QueueName == null) return;

        var eventInstance = new T { Message = $"Data from Publisher {Id}" };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventInstance));

        await Channel.BasicPublishAsync(
            exchange: string.Empty, 
            routingKey: QueueName, 
            body: body);

        LoggerHelper.LogCall($"Publisher {Id} sent {typeof(T).Name} to {QueueName}");
    }
}