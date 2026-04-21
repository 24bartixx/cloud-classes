using MediatR;
using Shared.Events;

namespace PlayerMatchesHistory.Application.Commands.SaveMatch;

public sealed record SaveMatchCommand(ClanWarEndedEvent EventData) : IRequest;