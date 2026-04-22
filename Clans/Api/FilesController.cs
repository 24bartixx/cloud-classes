using Clans.Application.Queries.DownloadFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clans.Api;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IMediator mediator, ILogger<FilesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("download/{fileMetadataId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadFile(
        Guid fileMetadataId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to download file with FileMetadataId={FileMetadataId}", fileMetadataId);

        var query = new DownloadFileQuery(fileMetadataId);
        var result = await _mediator.Send(query, cancellationToken);

        return File(
            fileStream: result.FileStream,
            contentType: result.ContentType,
            fileDownloadName: result.OriginalFileName,
            enableRangeProcessing: true);
    }
}
