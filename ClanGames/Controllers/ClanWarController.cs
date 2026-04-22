using MediatR;
using ClanGames.Application.Commands.PublishFileMessage;
using ClanGames.Application.Commands.PublishClanWarEnded;
using Microsoft.AspNetCore.Mvc;
using Shared.Events;

namespace ClanWars.Api.Controllers;

[ApiController]
[Route("api/clan-wars")]
public sealed class ClanWarController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ClanWarController> _logger;

    public ClanWarController(
        IMediator mediator,
        ILogger<ClanWarController> logger)
    {
        _mediator = mediator;
        _logger    = logger;
    }

    [HttpPost("end")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EndClanWar([FromBody] EndClanWarRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);


        _logger.LogInformation(
            "Ending Clan War {ClanWarId} with {ClanCount} clans and {PlayerCount} players.",
            request.ClanWarId, request.ClanResults.Count, request.PlayerStats.Count);

        await _mediator.Send(new PublishFileMessageCommand(), cancellationToken);

        var @event = new ClanWarEndedEvent
        {
            ClanWarId    = request.ClanWarId,
            EndedAtUtc   = DateTime.UtcNow,
            ClanResults  = request.ClanResults,
            PlayerStats  = request.PlayerStats
        };

        await _mediator.Send(new PublishClanWarEndedCommand(@event), cancellationToken);

        return Accepted(new { @event.ClanWarId, @event.EndedAtUtc });
    }
}

public sealed record EndClanWarRequest
{
    public Guid ClanWarId { get; init; } = Guid.NewGuid();
    public List<ClanResultDto> ClanResults { get; init; } = [];
    public List<PlayerStatsDto> PlayerStats { get; init; } = [];
}
