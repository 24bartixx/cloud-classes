using RabbitMQ.Client;
using MediatR;
using Rewards.Infrastructure.Configuration;
using Rewards.Infrastructure.Bus;
using Rewards.Application.Commands.ProcessClanWarEnded;
using Shared.Events;

namespace Rewards.Service.Workers;

public sealed class RewardsWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IMediator _mediator;
    private readonly ILogger<RewardsWorker> _logger;
    private readonly RabbitMqSettings _settings;

    public RewardsWorker(
        IMessageConsumer consumer,
        IMediator mediator,
        ILogger<RewardsWorker> logger,
        RabbitMqSettings settings)
    {
        _consumer  = consumer;
        _mediator  = mediator;
        _logger    = logger;
        _settings  = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Rewards.Service starting — binding queue '{Queue}' to exchange '{Exchange}'.",
            QueueNames.RewardsWarEnded, ExchangeNames.ClanWarEnded);

        BindQueueToFanoutExchange(
            QueueNames.RewardsWarEnded,
            ExchangeNames.ClanWarEnded);

        await _consumer.StartConsumingAsync<ClanWarEndedEvent>(
            QueueNames.RewardsWarEnded,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(ClanWarEndedEvent @event)
    {
        await _mediator.Send(new ProcessClanWarEndedCommand(@event));
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
