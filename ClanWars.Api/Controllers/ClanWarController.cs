using Microsoft.AspNetCore.Mvc;
using TanksGame.Shared.Configuration;
using TanksGame.Shared.Events;
using TanksGame.Shared.Messaging;

namespace ClanWars.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPost("end-clan-war")]
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

        _logger.LogInformation(
            "ClanWarEndedEvent published to exchange '{Exchange}'.",
            ExchangeNames.ClanWarEnded);

        return Accepted(new { @event.ClanWarId, @event.EndedAtUtc });
    }
}

public sealed record EndClanWarRequest
{
    public Guid ClanWarId { get; init; } = Guid.NewGuid();
    public List<ClanResult> ClanResults { get; init; } = [];
    public List<PlayerStats> PlayerStats { get; init; } = [];
}
