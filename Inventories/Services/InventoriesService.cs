using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Shared.Events;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Inventories.Service.Services;

public sealed class InventoriesService : IInventoriesService
{
    private readonly IMongoCollection<BsonDocument> _collection;
    private readonly ILogger<InventoriesService> _logger;

    public InventoriesService(IConfiguration configuration, ILogger<InventoriesService> logger)
    {
        var connectionString = configuration["MongoDb:ConnectionString"];
        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(mongoUrl);
        var db = client.GetDatabase(mongoUrl.DatabaseName ?? "inventories");
        _collection = db.GetCollection<BsonDocument>("inventories");
        _logger = logger;
    }

    public async Task UpsertInventoryAsync(InventoryUpdatedEvent @event, CancellationToken ct = default)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("playerId", @event.PlayerId.ToString());
        var update = Builders<BsonDocument>.Update
            .Inc("experience", @event.GrantedItems.Sum(i => i.Quantity)) 
            .Inc("credits", 0) 
            .AddToSetEach("tanks", @event.GrantedItems.Where(i => i.ItemType == "tank").Select(i => i.ItemName));

        var updateDef = new List<UpdateDefinition<BsonDocument>>
        {
            update,
            Builders<BsonDocument>.Update.Set("playerName", @event.PlayerName)
        };

        var result = await _collection.UpdateOneAsync(
            filter,
            Builders<BsonDocument>.Update.Combine(updateDef),
            new UpdateOptions { IsUpsert = true },
            ct
        );

        _logger.LogInformation("Upserted inventory for PlayerId={PlayerId}, Matched={MatchedCount}, Modified={ModifiedCount}",
            @event.PlayerId, result.MatchedCount, result.ModifiedCount);
    }
}
