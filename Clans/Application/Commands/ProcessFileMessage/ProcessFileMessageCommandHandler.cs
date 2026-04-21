using MediatR;

namespace Clans.Application.Commands.ProcessFileMessage;

public sealed class ProcessFileMessageCommandHandler : IRequestHandler<ProcessFileMessageCommand>
{
    private readonly ILogger<ProcessFileMessageCommandHandler> _logger;

    public ProcessFileMessageCommandHandler(ILogger<ProcessFileMessageCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProcessFileMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing FileMessageEvent for FileName={FileName}",
            request.EventData.FileName);

        return Task.CompletedTask;
    }
}