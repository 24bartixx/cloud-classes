using System.Text;
using System.Text.Json;
using PlayerMatchesHistory.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlayerMatchesHistory.Infrastructure.Messaging;

public sealed class RabbitMqConsumer : IMessageConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private bool _disposed;

    public RabbitMqConsumer(RabbitMqSettings settings, ILogger<RabbitMqConsumer> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(settings.ConnectionUrl),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval  = TimeSpan.FromSeconds(10),
            DispatchConsumersAsync   = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 50, global: false);

        _logger.LogInformation("RabbitMqConsumer initialised.");
    }

    public Task StartConsumingAsync<T>(
        string queue,
        Func<T, Task> handler,
        CancellationToken cancellationToken = default) where T : class
    {
        _channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            try
            {
                var message = JsonSerializer.Deserialize<T>(json)
                    ?? throw new InvalidOperationException($"Deserialised null from queue '{queue}'.");
                _logger.LogInformation("Received {MessageType} from queue '{Queue}'.", typeof(T).Name, queue);
                await handler(message);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing {MessageType} from queue '{Queue}'.", typeof(T).Name, queue);
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
        _logger.LogInformation("Started consuming from queue '{Queue}'.", queue);
        return Task.Delay(Timeout.Infinite, cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _channel.Close();
        _connection.Close();
        _disposed = true;
    }
}
