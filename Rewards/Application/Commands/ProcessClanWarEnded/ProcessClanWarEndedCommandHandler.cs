using MediatR;
using Microsoft.EntityFrameworkCore;
using Rewards.Domain.Entities;
using Rewards.Infrastructure.Bus;
using Rewards.Infrastructure.Configuration;
using Rewards.Service.Infrastructure.Persistence;
using Shared.Events;

namespace Rewards.Application.Commands.ProcessClanWarEnded;

public sealed class ProcessClanWarEndedCommandHandler : IRequestHandler<ProcessClanWarEndedCommand>
{
    private readonly RewardsDbContext _dbContext;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<ProcessClanWarEndedCommandHandler> _logger;

    public ProcessClanWarEndedCommandHandler(
        RewardsDbContext dbContext,
        IMessagePublisher publisher,
        ILogger<ProcessClanWarEndedCommandHandler> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Handle(ProcessClanWarEndedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Selecting rewards for ClanWarId={ClanWarId} - {Count} players.",
            request.EventData.ClanWarId,
            request.EventData.PlayerStats.Count);

        var rewards = await _dbContext.Rewards
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var player in request.EventData.PlayerStats)
        {
            var selectedReward = SelectRewardForPlayer(player, rewards);

            var rewardEvent = new RewardSelectedEvent
            {
                ClanWarId = request.EventData.ClanWarId,
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                Reward = selectedReward
            };

            _publisher.PublishToQueue(QueueNames.InventoriesRewardSelected, rewardEvent);
        }
    }

    private static RewardItemDto SelectRewardForPlayer(PlayerStatsDto player, IReadOnlyList<Reward> rewards)
    {
        var matched = rewards.Where(r =>
            (!r.MinKills.HasValue || player.Kills >= r.MinKills) &&
            (!r.MaxDeaths.HasValue || player.Deaths <= r.MaxDeaths) &&
            (!r.MinDamageDealt.HasValue || player.DamageDealt >= r.MinDamageDealt) &&
            (!r.MaxDamageReceived.HasValue || player.DamageReceived <= r.MaxDamageReceived) &&
            (!r.MinSurvived.HasValue || (player.Survived ? 1 : 0) >= r.MinSurvived) &&
            (!r.MinExperienceEarned.HasValue || player.ExperienceEarned >= r.MinExperienceEarned)
        ).ToList();

        var totalCredits = matched.Sum(r => r.Credits);
        var totalExp = matched.Sum(r => r.Experience);
        var tanks = matched
            .Where(r => !string.IsNullOrWhiteSpace(r.TankId))
            .Select(r => r.TankId!)
            .Distinct()
            .ToList();

        return new RewardItemDto
        {
            Credits = totalCredits,
            Experience = totalExp,
            Tanks = tanks
        };
    }
}