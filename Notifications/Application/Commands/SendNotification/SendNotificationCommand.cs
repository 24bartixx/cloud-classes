using MediatR;
using Shared.Events;

namespace Notifications.Application.Commands.SendNotification;

public sealed record SendNotificationCommand(InventoryUpdatedEvent EventData) : IRequest<Guid>;