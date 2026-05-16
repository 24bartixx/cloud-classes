output "repository_names" {
  description = "Names of the ECR repositories."
  value       = { for name, repository in aws_ecr_repository.this : name => repository.name }
}

output "repository_arns" {
  description = "ARNs of the ECR repositories."
  value       = { for name, repository in aws_ecr_repository.this : name => repository.arn }
}

output "repository_urls" {
  description = "Repository URLs for pushing Docker images."
  value       = { for name, repository in aws_ecr_repository.this : name => repository.repository_url }
}
