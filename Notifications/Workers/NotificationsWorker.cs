using Notifications.Infrastructure.Configuration;
using Notifications.Infrastructure.Bus;
using MediatR;
using Notifications.Application.Commands.SendNotification;
using Shared.Events;

namespace Notifications.Service.Workers;

public sealed class NotificationsWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationsWorker> _logger;

    public NotificationsWorker(
        IMessageConsumer consumer,
        IMediator mediator,
        ILogger<NotificationsWorker> logger)
    {
        _consumer = consumer;
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Notifications.Service starting — consuming from queue '{Queue}'.",
            QueueNames.NotificationsInventoryUpdated);

        await _consumer.StartConsumingAsync<InventoryUpdatedEvent>(
            QueueNames.NotificationsInventoryUpdated,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(InventoryUpdatedEvent @event)
    {
        await _mediator.Send(new SendNotificationCommand(@event));
    }
}
