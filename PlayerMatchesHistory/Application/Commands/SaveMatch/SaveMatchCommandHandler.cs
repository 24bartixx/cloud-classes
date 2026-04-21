using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PlayerMatchesHistory.Application.Commands.SaveMatch;

public sealed class SaveMatchCommandHandler : IRequestHandler<SaveMatchCommand>
{
    private readonly IMongoCollection<BsonDocument> _collection;
    private readonly ILogger<SaveMatchCommandHandler> _logger;

    public SaveMatchCommandHandler(IConfiguration configuration, ILogger<SaveMatchCommandHandler> logger)
    {
        var connectionString = configuration["MongoDb:ConnectionString"];
        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(mongoUrl);
        var db = client.GetDatabase(mongoUrl.DatabaseName ?? "playermatches");

        _collection = db.GetCollection<BsonDocument>("playermatches");
        _logger = logger;
    }

    public async Task Handle(SaveMatchCommand request, CancellationToken cancellationToken)
    {
        var matchEvent = request.EventData;

        _logger.LogInformation(
            "Storing match history for ClanWarId={ClanWarId} - {Count} player records.",
            matchEvent.ClanWarId,
            matchEvent.PlayerStats.Count);

        var docs = matchEvent.PlayerStats.Select(player => new BsonDocument
        {
            { "gameId", matchEvent.ClanWarId.ToString() },
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

        await _collection.InsertManyAsync(docs, cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Saved {Count} player stats for ClanWar {ClanWarId}",
            matchEvent.PlayerStats.Count,
            matchEvent.ClanWarId);
    }
}