namespace Clans.Domain.Entities;

public class FileMetadata
{
    public Guid FileMetadataId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string S3BucketName { get; set; } = string.Empty;
    public string S3ObjectKey { get; set; } = string.Empty;
}
