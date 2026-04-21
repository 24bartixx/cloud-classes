using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using Inventories.Infrastructure.Bus;
using Inventories.Infrastructure.Configuration;
using Shared.Events;

namespace Inventories.Application.Commands.UpdateInventory
{
    public sealed class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand>
    {
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly IMessagePublisher _publisher;
        private readonly ILogger<UpdateInventoryCommandHandler> _logger;

        public UpdateInventoryCommandHandler(
            IConfiguration configuration,
            IMessagePublisher publisher,
            ILogger<UpdateInventoryCommandHandler> logger)
        {
            var connectionString = configuration["MongoDb:ConnectionString"];
            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            var db = client.GetDatabase(mongoUrl.DatabaseName ?? "inventories");

            _collection = db.GetCollection<BsonDocument>("inventories");
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
        {
            var rewardSelectedEvent = request.EventData;
            var reward = rewardSelectedEvent.Reward;

            var rewardSummary = $"Credits: {reward.Credits}, Experience: {reward.Experience}";
            if (reward.Tanks.Count > 0)
            {
                rewardSummary += $", Tanks: {string.Join(", ", reward.Tanks)}";
            }

            _logger.LogInformation(
                "Granting reward [{RewardSummary}] to Player '{PlayerName}' (Id={PlayerId}).",
                rewardSummary,
                rewardSelectedEvent.PlayerName,
                rewardSelectedEvent.PlayerId);

            var inventoryUpdatedEvent = new InventoryUpdatedEvent
            {
                PlayerId = rewardSelectedEvent.PlayerId,
                PlayerName = rewardSelectedEvent.PlayerName,
                Reward = rewardSelectedEvent.Reward,
                UpdatedAtUtc = DateTime.UtcNow
            };

            var filter = Builders<BsonDocument>.Filter.Eq("playerId", inventoryUpdatedEvent.PlayerId.ToString());
            var update = Builders<BsonDocument>.Update
                .Inc("experience", inventoryUpdatedEvent.Reward.Experience)
                .Inc("credits", inventoryUpdatedEvent.Reward.Credits)
                .AddToSetEach("tanks", inventoryUpdatedEvent.Reward.Tanks);

            var updateDef = new List<UpdateDefinition<BsonDocument>>
            {
                update,
                Builders<BsonDocument>.Update.Set("playerName", inventoryUpdatedEvent.PlayerName)
            };

            await _collection.UpdateOneAsync(
                filter,
                Builders<BsonDocument>.Update.Combine(updateDef),
                new UpdateOptions { IsUpsert = true },
                cancellationToken);

            _logger.LogInformation("Upserted inventory for PlayerId={PlayerId}", inventoryUpdatedEvent.PlayerId);

            _publisher.PublishToQueue(
                QueueNames.NotificationsInventoryUpdated,
                inventoryUpdatedEvent);
        }
    }
}