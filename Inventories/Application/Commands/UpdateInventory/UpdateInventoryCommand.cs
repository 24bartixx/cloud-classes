using MediatR;
using Shared.Events;

namespace Inventories.Application.Commands.UpdateInventory;

public sealed record UpdateInventoryCommand(RewardSelectedEvent EventData) : IRequest;