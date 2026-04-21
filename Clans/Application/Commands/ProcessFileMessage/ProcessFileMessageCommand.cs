using MediatR;
using Shared.Events;

namespace Clans.Application.Commands.ProcessFileMessage;

public sealed record ProcessFileMessageCommand(FileMessageEvent EventData) : IRequest;