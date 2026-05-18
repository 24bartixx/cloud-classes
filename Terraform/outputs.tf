output "inventories_dynamodb_table_name" {
  description = "Name of the inventories DynamoDB table."
  value       = module.dynamodb.inventories_table_name
}

output "inventories_dynamodb_table_arn" {
  description = "ARN of the inventories DynamoDB table."
  value       = module.dynamodb.inventories_table_arn
}

output "player_matches_history_dynamodb_table_name" {
  description = "Name of the player matches history DynamoDB table."
  value       = module.dynamodb.player_matches_history_table_name
}

output "player_matches_history_dynamodb_table_arn" {
  description = "ARN of the player matches history DynamoDB table."
  value       = module.dynamodb.player_matches_history_table_arn
}

output "ecr_repository_names" {
  description = "Names of the ECR repositories."
  value       = module.ecr.repository_names
}

output "ecr_repository_arns" {
  description = "ARNs of the ECR repositories."
  value       = module.ecr.repository_arns
}

output "ecr_repository_urls" {
  description = "Repository URLs for pushing Docker images."
  value       = module.ecr.repository_urls
}

output "rds_postgres_security_group_id" {
  description = "ID of the PostgreSQL RDS security group."
  value       = module.database.rds_postgres_security_group_id
}

output "rds_postgres_security_group_arn" {
  description = "ARN of the PostgreSQL RDS security group."
  value       = module.database.rds_postgres_security_group_arn
}

output "rds_postgres_instance_id" {
  description = "ID of the PostgreSQL RDS instance."
  value       = module.database.rds_postgres_instance_id
}

output "rds_postgres_instance_arn" {
  description = "ARN of the PostgreSQL RDS instance."
  value       = module.database.rds_postgres_instance_arn
}

output "rds_postgres_endpoint" {
  description = "Connection endpoint of the PostgreSQL RDS instance."
  value       = module.database.rds_postgres_endpoint
}

output "rds_postgres_address" {
  description = "Hostname of the PostgreSQL RDS instance."
  value       = module.database.rds_postgres_address
}

output "rds_postgres_port" {
  description = "Port of the PostgreSQL RDS instance."
  value       = module.database.rds_postgres_port
}

output "rds_postgres_master_user_secret_arn" {
  description = "ARN of the AWS-managed Secrets Manager secret for the PostgreSQL master user."
  value       = module.database.rds_postgres_master_user_secret_arn
}

output "ecs_cluster_name" {
  description = "Name of the ECS cluster."
  value       = module.ecs.cluster_name
}

output "ecs_cluster_arn" {
  description = "ARN of the ECS cluster."
  value       = module.ecs.cluster_arn
}

output "ecs_task_security_group_id" {
  description = "ID of the ECS task security group."
  value       = module.ecs.task_security_group_id
}

output "ecs_service_names" {
  description = "Names of the ECS services."
  value       = module.ecs.service_names
}

output "ecs_task_definition_arns" {
  description = "ARNs of the ECS task definitions."
  value       = module.ecs.task_definition_arns
}
