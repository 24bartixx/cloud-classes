using ClanGames.Infrastructure.Configuration;
using ClanGames.Application;
using Microsoft.AspNetCore.Mvc;
using Shared.Events;

namespace ClanWars.Api.Controllers;

[ApiController]
[Route("api/clan-wars")]
public sealed class ClanWarController : ControllerBase
{
    private readonly IClanGamesService _clanGamesService;
    private readonly ILogger<ClanWarController> _logger;

    public ClanWarController(
        IClanGamesService clanGamesService,
        ILogger<ClanWarController> logger)
    {
        _clanGamesService = clanGamesService;
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

        var logFilePath = Path.Combine("Data", "mock_game.log");
        var resolvedPath = Path.GetFullPath(logFilePath);
        

        if (System.IO.File.Exists(resolvedPath))
        {
            var logBytes = ClanGames.Utils.ByteArrConverter.FileToByteArray(resolvedPath);
            var fileName = Path.GetFileName(resolvedPath);
            var fileExt = Path.GetExtension(resolvedPath);
            _logger.LogInformation("Received file: {FileName}, extension: {FileExt}, size: {Size} bytes", fileName, fileExt, logBytes.Length);

            var fileMessage = new FileMessageEvent
            {
                FileName = fileName,
                FileExtension = fileExt,
                Content = logBytes
            };
            _clanGamesService.PublishFileMessageEvent(fileMessage);
        }
        else
        {
            _logger.LogWarning("Mock game log file not found at path: {Path}", resolvedPath);
        }

        var @event = new ClanWarEndedEvent
        {
            ClanWarId    = request.ClanWarId,
            EndedAtUtc   = DateTime.UtcNow,
            ClanResults  = request.ClanResults,
            PlayerStats  = request.PlayerStats
        };

        _clanGamesService.PublishClanWarEndedEvent(@event);

        return Accepted(new { @event.ClanWarId, @event.EndedAtUtc });
    }
}

public sealed record EndClanWarRequest
{
    public Guid ClanWarId { get; init; } = Guid.NewGuid();
    public List<ClanResultDto> ClanResults { get; init; } = [];
    public List<PlayerStatsDto> PlayerStats { get; init; } = [];
}
