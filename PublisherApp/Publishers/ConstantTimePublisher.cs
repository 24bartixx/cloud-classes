using Shared;

namespace PublisherApp.Publishers;

public class ConstantTimePublisher:  BasePublisher<Type1Event>
{
    private readonly System.Timers.Timer _timer;
    
    public ConstantTimePublisher(int id, double intervalMs, RabbitBrokerClient brokerClient)
        : base(id, brokerClient)
    {
        _timer = new System.Timers.Timer(intervalMs);
        _timer.Elapsed += async (s, e) => await PublishEventAsync();
    }

    public override async Task RunAsync(CancellationToken ct = default)
    {
        await base.RunAsync(ct);
        _timer.Start();
    }

}