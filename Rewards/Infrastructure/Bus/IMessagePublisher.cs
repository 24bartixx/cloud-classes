namespace Rewards.Infrastructure.Bus;

public interface IMessagePublisher : IDisposable
{
    void PublishToExchange<T>(string exchange, T message) where T : class;
    void PublishToQueue<T>(string queue, T message) where T : class;
}
