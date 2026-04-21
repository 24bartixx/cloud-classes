using ClanGames.Infrastructure.Bus;
using ClanGames.Infrastructure.Configuration;
using MediatR;

namespace ClanGames.Application.Commands.PublishFileMessage;

public sealed class PublishFileMessageCommandHandler : IRequestHandler<PublishFileMessageCommand>
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<PublishFileMessageCommandHandler> _logger;

    public PublishFileMessageCommandHandler(
        IMessagePublisher publisher,
        ILogger<PublishFileMessageCommandHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public Task Handle(PublishFileMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing FileMessageEvent for FileName={FileName}",
            request.FileMessage.FileName);

        _publisher.PublishToQueue(QueueNames.FileUploaded, request.FileMessage);
        return Task.CompletedTask;
    }
}