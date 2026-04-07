using System.Text;
using System.Text.Json;
using ClanGames.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ClanGames.Infrastructure.Bus;

public sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private bool _disposed;

    public RabbitMqPublisher(RabbitMqSettings settings, ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(settings.ConnectionUrl),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection();
        _channel    = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: ExchangeNames.ClanWarEnded,
            type: RabbitMQ.Client.ExchangeType.Fanout,
            durable: true,
            autoDelete: false);

        DeclareQueue(QueueNames.FileUploaded);

        DeclareQueue(QueueNames.InventoriesRewardSelected);
        DeclareQueue(QueueNames.NotificationsInventoryUpdated);

        _logger.LogInformation("RabbitMqPublisher initialised and topology declared.");
    }

    public void PublishToExchange<T>(string exchange, T message) where T : class
    {
        var body  = Serialize(message);
        var props = BasicProps();
        _channel.BasicPublish(exchange: exchange, routingKey: string.Empty, basicProperties: props, body: body);
        _logger.LogInformation("Published {MessageType} to exchange '{Exchange}'.", typeof(T).Name, exchange);
    }

    public void PublishToQueue<T>(string queue, T message) where T : class
    {
        var body  = Serialize(message);
        var props = BasicProps();
        _channel.BasicPublish(exchange: string.Empty, routingKey: queue, basicProperties: props, body: body);
        _logger.LogInformation("Published {MessageType} to queue '{Queue}'.", typeof(T).Name, queue);
    }

    private void DeclareQueue(string name)
    {
        _channel.QueueDeclare(queue: name, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    private IBasicProperties BasicProps()
    {
        var props = _channel.CreateBasicProperties();
        props.Persistent      = true;
        props.ContentType     = "application/json";
        props.ContentEncoding = "UTF-8";
        return props;
    }

    private static ReadOnlyMemory<byte> Serialize<T>(T message) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

    public void Dispose()
    {
        if (_disposed) return;
        _channel.Close();
        _connection.Close();
        _disposed = true;
    }
}
