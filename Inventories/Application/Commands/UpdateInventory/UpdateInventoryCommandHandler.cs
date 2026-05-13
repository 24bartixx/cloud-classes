using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MediatR;
using Inventories.Infrastructure.Bus;
using Inventories.Infrastructure.Configuration;
using Shared.Events;

namespace Inventories.Application.Commands.UpdateInventory
{
    public sealed class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand>
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName;
        private readonly IMessagePublisher _publisher;
        
        private readonly ILogger<UpdateInventoryCommandHandler> _logger;

        public UpdateInventoryCommandHandler(
            IAmazonDynamoDB dynamoDb,
            IConfiguration configuration,
            IMessagePublisher publisher,
            ILogger<UpdateInventoryCommandHandler> logger)
        {
            _dynamoDb = dynamoDb;
            _tableName = configuration["DynamoDb:TableName"]
                ?? throw new InvalidOperationException("DynamoDb:TableName configuration is missing.");
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

            var playerId = inventoryUpdatedEvent.PlayerId.ToString();
            var expressionAttributeNames = new Dictionary<string, string>
            {
                ["#playerId"] = "playerId",
                ["#experience"] = "experience",
                ["#credits"] = "credits"
            };

            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":playerId"] = new() { S = playerId },
                [":experience"] = new() { N = inventoryUpdatedEvent.Reward.Experience.ToString() },
                [":credits"] = new() { N = inventoryUpdatedEvent.Reward.Credits.ToString() }
            };

            var updateExpression = "SET #playerId = :playerId ADD #experience :experience, #credits :credits";

            var tanks = inventoryUpdatedEvent.Reward.Tanks
                .Where(tank => !string.IsNullOrWhiteSpace(tank))
                .Distinct()
                .ToList();

            if (tanks.Count > 0)
            {
                expressionAttributeNames["#tanks"] = "tanks";
                expressionAttributeValues[":tanks"] = new AttributeValue { SS = tanks };
                updateExpression += ", #tanks :tanks";
            }

            await _dynamoDb.UpdateItemAsync(
                new UpdateItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["PK"] = new() { S = playerId },
                        ["SK"] = new() { S = "inventory" }
                    },
                    UpdateExpression = updateExpression,
                    ExpressionAttributeNames = expressionAttributeNames,
                    ExpressionAttributeValues = expressionAttributeValues
                },
                cancellationToken);

            _logger.LogInformation(
                "Upserted inventory for PlayerId={PlayerId} to DynamoDB table {TableName}",
                inventoryUpdatedEvent.PlayerId,
                _tableName);

            _publisher.PublishToQueue(
                QueueNames.NotificationsInventoryUpdated,
                inventoryUpdatedEvent);
        }
    }
}
