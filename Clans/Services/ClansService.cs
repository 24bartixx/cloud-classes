using Clans.Domain.Entities;
using Clans.Service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Events;

namespace Clans.Service.Services;

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

            // 2. Save individual Clan Results
            var clanIdsFromEvent = @event.ClanResults.Select(r => r.ClanId).Distinct().ToList();
            var existingClanIds = await context.Clans
                .Where(c => clanIdsFromEvent.Contains(c.ClanId))
                .Select(c => c.ClanId)
                .ToListAsync(ct);

            foreach (var resultDto in @event.ClanResults)
            {
                if (!existingClanIds.Contains(resultDto.ClanId))
                {
                    _logger.LogWarning("ClanId={ClanId} does not exist in database. Skipping result for ClanWarId={ClanWarId}.", 
                        resultDto.ClanId, @event.ClanWarId);
                    continue;
                }

                var clanResult = new ClanResult
                {
                    ClanResultId = Guid.NewGuid(),
                    ClanId = resultDto.ClanId,
                    ClanWarId = @event.ClanWarId,
                    Placement = resultDto.Placement,
                    Score = resultDto.Score
                };

                context.ClanResults.Add(clanResult);
            }

            await context.SaveChangesAsync(ct);
        }

        _logger.LogInformation("Successfully saved results for ClanWarId={ClanWarId}", @event.ClanWarId);
    }
}
