using MediatR;
using Shared.Events;

namespace ClanGames.Application.Commands.PublishFileMessage;

public sealed record PublishFileMessageCommand(FileMessageEvent FileMessage) : IRequest;