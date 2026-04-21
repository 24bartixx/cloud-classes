using Clans.Infrastructure.Bus;
using Clans.Infrastructure.Configuration;
using Clans.Application;
using RabbitMQ.Client;
using Shared.Events;

namespace Clans.Service.Workers;

public sealed class ClansWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IClansService _clansService;
    private readonly ILogger<ClansWorker> _logger;
    private readonly RabbitMqSettings _settings;

    public ClansWorker(
        IMessageConsumer consumer,
        IClansService clansService,
        ILogger<ClansWorker> logger,
        RabbitMqSettings settings)
    {
        _consumer = consumer;
        _clansService = clansService;
        _logger   = logger;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Clans.Service starting — binding queue '{Queue}' to exchange '{Exchange}'.",
            QueueNames.ClansWarEnded, ExchangeNames.ClanWarEnded);

        BindQueueToFanoutExchange(
            QueueNames.ClansWarEnded,
            ExchangeNames.ClanWarEnded);

        _logger.LogInformation("Clans.Service starting — consuming queue '{Queue}' for FileMessageEvent.", QueueNames.FileUploaded);

        var clanWarTask = _consumer.StartConsumingAsync<ClanWarEndedEvent>(
            QueueNames.ClansWarEnded,
            HandleAsync,
            stoppingToken);

        var fileMessageTask = _consumer.StartConsumingAsync<FileMessageEvent>(
            QueueNames.FileUploaded,
            HandleFileMessageAsync,
            stoppingToken);

        await Task.WhenAll(clanWarTask, fileMessageTask);
    }

    private async Task HandleAsync(ClanWarEndedEvent @event)
    {
        await _clansService.ProcessClanWarEndedEventAsync(@event);
    }

    private async Task HandleFileMessageAsync(FileMessageEvent @event)
    {
        await _clansService.ProcessFileMessageEventAsync(@event);
    }

    private void BindQueueToFanoutExchange(string queue, string exchange)
    {
        var factory = new ConnectionFactory { Uri = new Uri(_settings.ConnectionUrl) };
        using var conn    = factory.CreateConnection();
        using var channel = conn.CreateModel();

        channel.ExchangeDeclare(exchange, ExchangeType.Fanout, durable: true);
        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue: queue, exchange: exchange, routingKey: string.Empty);

        _logger.LogInformation("Queue '{Queue}' bound to exchange '{Exchange}'.", queue, exchange);
    }
}
