using Inventories.Infrastructure.Configuration;
using Inventories.Infrastructure.Bus;
using MediatR;
using Inventories.Application.Commands.UpdateInventory;
using Shared.Events;

namespace Inventories.Service.Workers;

public sealed class InventoriesWorker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IMediator _mediator;
    private readonly ILogger<InventoriesWorker> _logger;

    public InventoriesWorker(
        IMessageConsumer consumer,
        IMediator mediator,
        ILogger<InventoriesWorker> logger)
    {
        _consumer  = consumer;
        _mediator  = mediator;
        _logger    = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Inventories.Service starting — consuming from queue '{Queue}'.",
            QueueNames.InventoriesRewardSelected);

        await _consumer.StartConsumingAsync<RewardSelectedEvent>(
            QueueNames.InventoriesRewardSelected,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(RewardSelectedEvent @event)
    {
        await _mediator.Send(new UpdateInventoryCommand(@event));
    }
}
