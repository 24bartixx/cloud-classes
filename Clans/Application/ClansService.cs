using Clans.Domain.Entities;
using Clans.Service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Events;

namespace Clans.Application;

public sealed class ClansService : IClansService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ClansService> _logger;

    public ClansService(IServiceScopeFactory scopeFactory, ILogger<ClansService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ProcessClanWarEndedEventAsync(ClanWarEndedEvent @event, CancellationToken ct = default)
    {
        _logger.LogInformation("Processing ClanWarEndedEvent for ClanWarId={ClanWarId}", @event.ClanWarId);

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ClansDbContext>();

            // 1. Save general War Result
            if (await context.ClanWarResults.AnyAsync(r => r.ClanWarId == @event.ClanWarId, ct))
            {
                _logger.LogWarning("ClanWarId={ClanWarId} already exists in database. Skipping.", @event.ClanWarId);
                return;
            }

            var warResult = new ClanWarResult
            {
                ClanWarId = @event.ClanWarId,
                FinishDate = @event.EndedAtUtc,
                TotalClans = @event.ClanResults.Count
            };
            context.ClanWarResults.Add(warResult);

            foreach (var clanResultDto in @event.ClanResults)
            {
                var clanResult = new ClanResult
                {
                    ClanResultId = Guid.NewGuid(),
                    ClanId = clanResultDto.ClanId,
                    ClanWarId = @event.ClanWarId,
                    Placement = clanResultDto.Placement,
                    Score = clanResultDto.Score
                };
                context.ClanResults.Add(clanResult);
            }

            await context.SaveChangesAsync(ct);
        }
    }

    public async Task ProcessFileMessageEventAsync(FileMessageEvent @event, CancellationToken ct = default)
    {
        // Implement file message processing logic here
        _logger.LogInformation("Processing FileMessageEvent for FileName={FileName}", @event.FileName);
    }
}
