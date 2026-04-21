using Shared.Events;

namespace ClanGames.Application;

public interface IClanGamesService
{
    void PublishClanWarEndedEvent(ClanWarEndedEvent @event);
    void PublishFileMessageEvent(FileMessageEvent fileMessage);
}
