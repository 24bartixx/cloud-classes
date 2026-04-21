using MediatR;
using Shared.Events;

namespace Clans.Application.Commands.ProcessClanWarEnded;

public sealed record ProcessClanWarEndedCommand(ClanWarEndedEvent EventData) : IRequest;