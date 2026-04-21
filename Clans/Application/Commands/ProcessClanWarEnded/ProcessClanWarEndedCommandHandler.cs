using Clans.Domain.Entities;
using Clans.Service.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clans.Application.Commands.ProcessClanWarEnded;

public sealed class ProcessClanWarEndedCommandHandler : IRequestHandler<ProcessClanWarEndedCommand>
{
    private readonly ClansDbContext _dbContext;
    private readonly ILogger<ProcessClanWarEndedCommandHandler> _logger;

    public ProcessClanWarEndedCommandHandler(
        ClansDbContext dbContext,
        ILogger<ProcessClanWarEndedCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(ProcessClanWarEndedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing ClanWarEndedEvent for ClanWarId={ClanWarId}",
            request.EventData.ClanWarId);

        if (await _dbContext.ClanWarResults.AnyAsync(r => r.ClanWarId == request.EventData.ClanWarId, cancellationToken))
        {
            _logger.LogWarning(
                "ClanWarId={ClanWarId} already exists in database. Skipping.",
                request.EventData.ClanWarId);
            return;
        }

        var warResult = new ClanWarResult
        {
            ClanWarId = request.EventData.ClanWarId,
            FinishDate = request.EventData.EndedAtUtc,
            TotalClans = request.EventData.ClanResults.Count
        };

        _dbContext.ClanWarResults.Add(warResult);

        foreach (var clanResultDto in request.EventData.ClanResults)
        {
            var clanResult = new ClanResult
            {
                ClanResultId = Guid.NewGuid(),
                ClanId = clanResultDto.ClanId,
                ClanWarId = request.EventData.ClanWarId,
                Placement = clanResultDto.Placement,
                Score = clanResultDto.Score
            };

            _dbContext.ClanResults.Add(clanResult);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}