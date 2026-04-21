using MediatR;
using Shared.Events;

namespace ClanGames.Application.Commands.PublishClanWarEnded;

public sealed record PublishClanWarEndedCommand(ClanWarEndedEvent EventData) : IRequest;