using ClanGames.Infrastructure.Bus;
using ClanGames.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Events;

namespace ClanGames.Application;

public class ClanGamesService : IClanGamesService
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<ClanGamesService> _logger;

    public ClanGamesService(IMessagePublisher publisher, ILogger<ClanGamesService> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public void PublishClanWarEndedEvent(ClanWarEndedEvent @event)
    {
        _logger.LogInformation("Publishing ClanWarEndedEvent for ClanWarId={ClanWarId}", @event.ClanWarId);
        _publisher.PublishToExchange(ExchangeNames.ClanWarEnded, @event);
    }

    public void PublishFileMessageEvent(FileMessageEvent fileMessage)
    {
        _logger.LogInformation("Publishing FileMessageEvent for FileName={FileName}", fileMessage.FileName);
        _publisher.PublishToQueue(QueueNames.FileUploaded, fileMessage);
    }
}
