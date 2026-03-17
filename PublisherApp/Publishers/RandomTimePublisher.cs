
using Shared;

namespace PublisherApp.Publishers;

public class RandomTimePublisher<T>(int id, RabbitBrokerClient brokerClient, int min = 1000, int max = 5000)
    : BasePublisher<T>(id, brokerClient)
    where T : CustomEvent, new()
{
    private readonly Random _rng = new();

    public override async Task RunAsync(CancellationToken ct = default)
    {
        await base.RunAsync(ct);

        while (!ct.IsCancellationRequested)
        {
            var delay = _rng.Next(min, max);
            await Task.Delay(delay, ct);
            _ = PublishEventAsync();
        }
    }
}