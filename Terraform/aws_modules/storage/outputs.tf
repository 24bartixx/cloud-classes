output "clan_wars_registry_bucket_name" {
  description = "Name of the clan wars registry S3 bucket."
  value       = aws_s3_bucket.clan_wars_registry.bucket
}

output "clan_wars_registry_bucket_arn" {
  description = "ARN of the clan wars registry S3 bucket."
  value       = aws_s3_bucket.clan_wars_registry.arn
}
