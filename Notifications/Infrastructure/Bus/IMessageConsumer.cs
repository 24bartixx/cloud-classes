namespace Notifications.Infrastructure.Bus;

public interface IMessageConsumer : IDisposable
{
    Task StartConsumingAsync<T>(
        string queue,
        Func<T, Task> handler,
        CancellationToken cancellationToken = default) where T : class;
}
