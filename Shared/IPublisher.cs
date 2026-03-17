namespace Shared;

public interface IPublisher
{
    Task PublishEventAsync();
    Task PublishEventAsync(CustomEvent customEvent);
}

