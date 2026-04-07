using MongoDB.Bson;
using MongoDB.Driver;
using PlayerMatchesHistory.Domain.Entities;
using Shared.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace PlayerMatchesHistory.Service.Services;

public sealed class PlayerMatchesHistoryService : IPlayerMatchesHistoryService
{
    private readonly IMongoCollection<BsonDocument> _collection;
    private readonly ILogger<PlayerMatchesHistoryService> _logger;

    public PlayerMatchesHistoryService(IConfiguration configuration, ILogger<PlayerMatchesHistoryService> logger)
    {
        var connectionString = configuration["MongoDb:ConnectionString"];
        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(mongoUrl);
        var db = client.GetDatabase(mongoUrl.DatabaseName ?? "playermatches");
        _collection = db.GetCollection<BsonDocument>("playermatches");
        _logger = logger;
    }

    public async Task SavePlayerStatsAsync(ClanWarEndedEvent @event, CancellationToken ct = default)
    {
        var docs = @event.PlayerStats.Select(player => new BsonDocument
        {
            { "gameId", @event.ClanWarId.ToString() },
            { "playerId", player.PlayerId.ToString() },
            { "playerName", player.PlayerName },
            { "clanId", player.ClanId.ToString() },
            { "kills", player.Kills },
            { "deaths", player.Deaths },
            { "damageDealt", player.DamageDealt },
            { "damageReceived", player.DamageReceived },
            { "survived", player.Survived },
            { "experienceEarned", player.ExperienceEarned }
        });
        if (docs.Any())
        {
            await _collection.InsertManyAsync(docs, cancellationToken: ct);
            _logger.LogInformation("Inserted {Count} player match records for ClanWarId={ClanWarId}", docs.Count(), @event.ClanWarId);
        }
        else
        {
            _logger.LogWarning("No player stats to insert for ClanWarId={ClanWarId}", @event.ClanWarId);
        }
    }
}
