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

output "rds_postgres_security_group_id" {
  description = "ID of the PostgreSQL RDS security group."
  value       = module.database.rds_postgres_security_group_id
}

output "rds_postgres_security_group_arn" {
  description = "ARN of the PostgreSQL RDS security group."
  value       = module.database.rds_postgres_security_group_arn
}
