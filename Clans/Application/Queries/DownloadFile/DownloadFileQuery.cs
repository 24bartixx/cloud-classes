using MediatR;

namespace Clans.Application.Queries.DownloadFile;

public sealed record DownloadFileQuery(Guid FileMetadataId) : IRequest<DownloadFileQueryResponse>;
