using MediatR;
using Shared.Events;

namespace Rewards.Application.Commands.ProcessClanWarEnded;

public sealed record ProcessClanWarEndedCommand(ClanWarEndedEvent EventData) : IRequest;