using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Rewards.Service.Infrastructure.Persistence;

public sealed class DatabaseConnectionLoggingInterceptor : DbConnectionInterceptor
{
    private readonly ILogger<DatabaseConnectionLoggingInterceptor> _logger;

    public DatabaseConnectionLoggingInterceptor(ILogger<DatabaseConnectionLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        LogConnectionStarting(connection);
        return base.ConnectionOpening(connection, eventData, result);
    }

    public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        LogConnectionStarting(connection);
        return base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }

    public override void ConnectionOpened(
        DbConnection connection,
        ConnectionEndEventData eventData)
    {
        LogConnectionResult(connection, succeeded: true, exception: null);
        base.ConnectionOpened(connection, eventData);
    }

    public override Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        LogConnectionResult(connection, succeeded: true, exception: null);
        return base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionFailed(
        DbConnection connection,
        ConnectionErrorEventData eventData)
    {
        LogConnectionResult(connection, succeeded: false, eventData.Exception);
        base.ConnectionFailed(connection, eventData);
    }

    public override Task ConnectionFailedAsync(
        DbConnection connection,
        ConnectionErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        LogConnectionResult(connection, succeeded: false, eventData.Exception);
        return base.ConnectionFailedAsync(connection, eventData, cancellationToken);
    }

    private void LogConnectionStarting(DbConnection connection)
    {
        _logger.LogInformation(
            "Starting database connection. Database={Database}, DataSource={DataSource}.",
            connection.Database,
            connection.DataSource);
    }

    private void LogConnectionResult(
        DbConnection connection,
        bool succeeded,
        Exception? exception)
    {
        if (succeeded)
        {
            _logger.LogInformation(
                "Database connection result: Success. Database={Database}, DataSource={DataSource}.",
                connection.Database,
                connection.DataSource);
            return;
        }

        _logger.LogError(
            exception,
            "Database connection result: Failure. Database={Database}, DataSource={DataSource}.",
            connection.Database,
            connection.DataSource);
    }
}
