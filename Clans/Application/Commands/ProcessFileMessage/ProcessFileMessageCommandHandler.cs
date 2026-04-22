using Amazon.S3;
using Amazon.S3.Model;
using MediatR;

namespace Clans.Application.Commands.ProcessFileMessage;

public sealed class ProcessFileMessageCommandHandler(
    ILogger<ProcessFileMessageCommandHandler> logger,
    IAmazonS3 s3Client) : IRequestHandler<ProcessFileMessageCommand>
{
    private const string BucketName = "pwr-ist-280462-tanks-517392773395-us-east-1-an";
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly ILogger<ProcessFileMessageCommandHandler> _logger = logger;
    
    public async Task Handle(ProcessFileMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing FileMessageEvent for FileName={FileName}, FileExtension={FileExtension}, FileSizeBytes={FileSizeBytes}, CreatedAtUtc={CreatedAtUtc}",
            request.EventData.FileName,
            request.EventData.FileExtension,
            request.EventData.FileSizeBytes,
            request.EventData.CreatedAtUtc);

        var normalizedExtension = request.EventData.FileExtension.TrimStart('.').ToLowerInvariant();
        var extensionSuffix = string.IsNullOrWhiteSpace(normalizedExtension) ? string.Empty : $".{normalizedExtension}";
        var objectKey = $"file-messages/{request.EventData.CreatedAtUtc:yyyy/MM/dd}/{Guid.NewGuid():N}-{request.EventData.FileName}{extensionSuffix}";

        await using var stream = new MemoryStream(request.EventData.Content);
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = objectKey,
            InputStream = stream,
            AutoCloseStream = false,
            ContentType = "application/octet-stream"
        };

        var response = await _s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
        _logger.LogInformation(
            "Uploaded file to S3. Bucket={BucketName}, Key={ObjectKey}, HttpStatusCode={HttpStatusCode}",
            BucketName,
            objectKey,
            response.HttpStatusCode);

    }
}