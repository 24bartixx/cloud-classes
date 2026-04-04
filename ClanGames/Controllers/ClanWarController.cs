using ClanGames.Infrastructure.Configuration;
using ClanGames.Infrastructure.Bus;
using Microsoft.AspNetCore.Mvc;
using Shared.Events;

namespace ClanWars.Api.Controllers;

[ApiController]
[Route("api/clan-wars")]
public sealed class ClanWarController : ControllerBase
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<ClanWarController> _logger;

    public ClanWarController(
        IMessagePublisher publisher,
        ILogger<ClanWarController> logger)
    {
        _publisher = publisher;
        _logger    = logger;
    }

    [HttpPost("end")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult EndClanWar([FromBody] EndClanWarRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation(
            "Ending Clan War {ClanWarId} with {ClanCount} clans and {PlayerCount} players.",
            request.ClanWarId, request.ClanResults.Count, request.PlayerStats.Count);

        var @event = new ClanWarEndedEvent
        {
            ClanWarId    = request.ClanWarId,
            EndedAtUtc   = DateTime.UtcNow,
            ClanResults  = request.ClanResults,
            PlayerStats  = request.PlayerStats
        };

        _publisher.PublishToExchange(ExchangeNames.ClanWarEnded, @event);

        return Accepted(new { @event.ClanWarId, @event.EndedAtUtc });
    }
}

public sealed record EndClanWarRequest
{
    public Guid ClanWarId { get; init; } = Guid.NewGuid();
    public List<ClanResultDto> ClanResults { get; init; } = [];
    public List<PlayerStatsDto> PlayerStats { get; init; } = [];
}
