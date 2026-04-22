namespace Clans.Application.Queries.DownloadFile;

public sealed record DownloadFileQueryResponse
{
    public Stream FileStream { get; init; } = Stream.Null;
    public string ContentType { get; init; } = "application/octet-stream";
    public string OriginalFileName { get; init; } = string.Empty;
}
