using Notifications.Infrastructure.Configuration;
using Notifications.Infrastructure.Bus;
using Notifications.Infrastructure.Persistence;
using Notifications.Domain.Entities;
using Notifications.Service.Services;
using Shared.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Notifications.Service.Workers;

public sealed class NotificationsWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly INotificationsService _notificationsService;
    private readonly ILogger<NotificationsWorker> _logger;

    public NotificationsWorker(
        IMessageConsumer consumer,
        INotificationsService notificationsService,
        ILogger<NotificationsWorker> logger)
    {
        _consumer = consumer;
        _notificationsService = notificationsService;
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
        await _notificationsService.ProcessInventoryUpdatedEventAsync(@event);
    }
}
