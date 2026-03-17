using RabbitMQ.Client;

namespace Shared;

public sealed class RabbitBrokerClient(string uri) : IAsyncDisposable
{
    private readonly ConnectionFactory _factory = new ConnectionFactory
    {
        Uri = new Uri(uri)
    };
    private IConnection? _connection;
    
    public async Task InitializeAsync()
    {
        _connection ??= await _factory.CreateConnectionAsync();
    }
    

    public async Task<IChannel> CreateChannelAsync()
    {
        if (_connection == null)
            throw new InvalidOperationException("InitializeAsync() should be called before creating channel!");
                
        return await _connection.CreateChannelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
    }
}