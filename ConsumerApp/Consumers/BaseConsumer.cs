using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;

namespace ConsumerApp.Consumers;

public abstract class BaseConsumer<T>(int id, RabbitBrokerClient brokerClient) 
    where T : CustomEvent, new()
{
    protected readonly int Id = id;
    protected IChannel? Channel;
    protected string? QueueName;

    public virtual async Task SubscribeAsync(CancellationToken ct = default)
    {
        Channel = await brokerClient.CreateChannelAsync();
        QueueName = BrokerReflection.GetQueueName<T>();
        
        await Channel.QueueDeclareAsync(
            queue: QueueName, 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            cancellationToken: ct);
        
        LoggerHelper.LogCall($"Consumer {id} initialized for queue {QueueName}");

        var consumer = new AsyncEventingBasicConsumer(Channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<T>(messageJson);

            if (message != null)
            {
                await OnEventReceived(message);
            }
        };

        await Channel.BasicConsumeAsync(queue: QueueName, autoAck: true, consumer: consumer, cancellationToken: ct);
    }
    
    protected abstract Task OnEventReceived(T receivedEvent);
}