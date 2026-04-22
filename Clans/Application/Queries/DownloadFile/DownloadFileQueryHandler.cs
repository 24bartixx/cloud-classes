using Amazon.S3;
using Amazon.S3.Model;
using Clans.Service.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clans.Application.Queries.DownloadFile;

public sealed class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, DownloadFileQueryResponse>
{
    private readonly ClansDbContext _dbContext;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<DownloadFileQueryHandler> _logger;

    public DownloadFileQueryHandler(
        ClansDbContext dbContext,
        IAmazonS3 s3Client,
        ILogger<DownloadFileQueryHandler> logger)
    {
        _dbContext = dbContext;
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task<DownloadFileQueryResponse> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading file with FileMetadataId={FileMetadataId}", request.FileMetadataId);

        var fileMetadata = await _dbContext.FileMetadata
            .FirstOrDefaultAsync(fm => fm.FileMetadataId == request.FileMetadataId, cancellationToken);

        if (fileMetadata == null)
        {
            throw new KeyNotFoundException($"FileMetadata with id {request.FileMetadataId} not found");
        }

        var getObjectRequest = new GetObjectRequest
        {
            BucketName = fileMetadata.S3BucketName,
            Key = fileMetadata.S3ObjectKey
        };

        var response = await _s3Client.GetObjectAsync(getObjectRequest, cancellationToken);

        _logger.LogInformation(
            "Retrieved file from S3. Bucket={BucketName}, Key={ObjectKey}, HttpStatusCode={HttpStatusCode}",
            fileMetadata.S3BucketName,
            fileMetadata.S3ObjectKey,
            response.HttpStatusCode);

        var contentType = string.IsNullOrWhiteSpace(fileMetadata.FileExtension) 
            ? "application/octet-stream" 
            : GetContentTypeFromExtension(fileMetadata.FileExtension);

        return new DownloadFileQueryResponse
        {
            FileStream = response.ResponseStream,
            ContentType = contentType,
            OriginalFileName = fileMetadata.FileName
        };
    }

    private static string GetContentTypeFromExtension(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            "pdf" => "application/pdf",
            "doc" or "docx" => "application/msword",
            "xls" or "xlsx" => "application/vnd.ms-excel",
            "txt" => "text/plain",
            "csv" => "text/csv",
            "json" => "application/json",
            "xml" => "application/xml",
            "log" => "text/plain",
            "png" => "image/png",
            "jpg" or "jpeg" => "image/jpeg",
            "gif" => "image/gif",
            _ => "application/octet-stream"
        };
    }
}
