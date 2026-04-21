using ClanGames.Infrastructure.Bus;
using ClanGames.Infrastructure.Configuration;
using MediatR;

namespace ClanGames.Application.Commands.PublishClanWarEnded;

public sealed class PublishClanWarEndedCommandHandler : IRequestHandler<PublishClanWarEndedCommand>
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<PublishClanWarEndedCommandHandler> _logger;

    public PublishClanWarEndedCommandHandler(
        IMessagePublisher publisher,
        ILogger<PublishClanWarEndedCommandHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public Task Handle(PublishClanWarEndedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing ClanWarEndedEvent for ClanWarId={ClanWarId}",
            request.EventData.ClanWarId);

        _publisher.PublishToExchange(ExchangeNames.ClanWarEnded, request.EventData);
        return Task.CompletedTask;
    }
}