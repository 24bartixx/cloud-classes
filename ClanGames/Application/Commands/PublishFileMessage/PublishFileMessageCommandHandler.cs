using ClanGames.Infrastructure.Bus;
using ClanGames.Infrastructure.Configuration;
using MediatR;
using Shared.Events;

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

    public async Task Handle(PublishFileMessageCommand request, CancellationToken cancellationToken)
    {
        var logFilePath = Path.Combine("Data", "mock_game.log");
        var resolvedPath = Path.GetFullPath(logFilePath);

        if (!File.Exists(resolvedPath))
        {
            _logger.LogWarning("Mock game log file not found at path: {Path}", resolvedPath);
            return;
        }

        var fileInfo = new FileInfo(resolvedPath);
        var logBytes = ClanGames.Utils.ByteArrConverter.FileToByteArray(resolvedPath);
        var fileName = fileInfo.Name;
        var fileExt = fileInfo.Extension;
        var createdAtUtc = fileInfo.CreationTimeUtc;

        _logger.LogInformation(
            "Publishing file: {FileName}, extension: {FileExt}, size: {Size} bytes, created at {CreatedAtUtc}",
            fileName, fileExt, fileInfo.Length, createdAtUtc);

        var fileMessage = new FileMessageEvent
        {
            FileName = fileName,
            FileExtension = fileExt,
            FileSizeBytes = fileInfo.Length,
            CreatedAtUtc = createdAtUtc,
            Content = logBytes
        };

        _publisher.PublishToQueue(QueueNames.FileUploaded, fileMessage);
        await Task.CompletedTask;
    }
}