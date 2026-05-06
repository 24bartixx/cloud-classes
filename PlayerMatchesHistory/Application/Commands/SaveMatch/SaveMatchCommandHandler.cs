using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MediatR;

namespace PlayerMatchesHistory.Application.Commands.SaveMatch;

public sealed class SaveMatchCommandHandler : IRequestHandler<SaveMatchCommand>
{
    private const int DynamoDbBatchWriteLimit = 25;
    private const int MaxBatchWriteRetries = 5;

    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;
    private readonly ILogger<SaveMatchCommandHandler> _logger;

    public SaveMatchCommandHandler(
        IAmazonDynamoDB dynamoDb,
        IConfiguration configuration,
        ILogger<SaveMatchCommandHandler> logger)
    {
        _dynamoDb = dynamoDb;
        _tableName = configuration["DynamoDb:TableName"]
            ?? throw new InvalidOperationException("DynamoDb:TableName configuration is missing.");
        _logger = logger;
    }

    public async Task Handle(SaveMatchCommand request, CancellationToken cancellationToken)
    {
        var matchEvent = request.EventData;

        _logger.LogInformation(
            "Storing match history for ClanWarId={ClanWarId} - {Count} player records.",
            matchEvent.ClanWarId,
            matchEvent.PlayerStats.Count);

        var writeRequests = matchEvent.PlayerStats
            .Select(player =>
            {
                var id = Guid.NewGuid().ToString();
                var gameId = matchEvent.ClanWarId.ToString();
                var playerId = player.PlayerId.ToString();

                return new WriteRequest
                {
                    PutRequest = new PutRequest
                    {
                        Item = new Dictionary<string, AttributeValue>
                        {
                            ["PK"] = new() { S = id },
                            ["SK"] = new() { S = playerId },
                            ["id"] = new() { S = id },
                            ["gameId"] = new() { S = gameId },
                            ["playerId"] = new() { S = playerId },
                            ["kills"] = new() { N = player.Kills.ToString() },
                            ["deaths"] = new() { N = player.Deaths.ToString() },
                            ["damageDealt"] = new() { N = player.DamageDealt.ToString() },
                            ["damageReceived"] = new() { N = player.DamageReceived.ToString() },
                            ["survived"] = new() { BOOL = player.Survived },
                            ["experienceEarned"] = new() { N = player.ExperienceEarned.ToString() }
                        }
                    }
                };
            })
            .ToList();

        foreach (var batch in writeRequests.Chunk(DynamoDbBatchWriteLimit))
        {
            await WriteBatchAsync(batch.ToList(), cancellationToken);
        }

        _logger.LogInformation(
            "Saved {Count} player stats for ClanWar {ClanWarId} to DynamoDB table {TableName}",
            matchEvent.PlayerStats.Count,
            matchEvent.ClanWarId,
            _tableName);
    }

    private async Task WriteBatchAsync(
        List<WriteRequest> writeRequests,
        CancellationToken cancellationToken)
    {
        var requestItems = new Dictionary<string, List<WriteRequest>>
        {
            [_tableName] = writeRequests
        };

        for (var attempt = 1; requestItems.Count > 0; attempt++)
        {
            _logger.LogInformation(
                "Starting database connection. Database=DynamoDB, TableName={TableName}, Attempt={Attempt}.",
                _tableName,
                attempt);

            BatchWriteItemResponse response;
            try
            {
                response = await _dynamoDb.BatchWriteItemAsync(
                    new BatchWriteItemRequest { RequestItems = requestItems },
                    cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Database connection result: Failure. Database=DynamoDB, TableName={TableName}, Attempt={Attempt}.",
                    _tableName,
                    attempt);
                throw;
            }

            _logger.LogInformation(
                "Database connection result: Success. Database=DynamoDB, TableName={TableName}, Attempt={Attempt}.",
                _tableName,
                attempt);

            requestItems = response.UnprocessedItems;
            if (requestItems.Count == 0)
            {
                return;
            }

            if (attempt >= MaxBatchWriteRetries)
            {
                throw new InvalidOperationException(
                    $"DynamoDB batch write left {requestItems.Sum(i => i.Value.Count)} unprocessed item(s) after {MaxBatchWriteRetries} attempts.");
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken);
        }
    }
}
