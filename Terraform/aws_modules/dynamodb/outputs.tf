output "inventories_table_name" {
  description = "Name of the inventories DynamoDB table."
  value       = aws_dynamodb_table.inventories.name
}

output "inventories_table_arn" {
  description = "ARN of the inventories DynamoDB table."
  value       = aws_dynamodb_table.inventories.arn
}

output "player_matches_history_table_name" {
  description = "Name of the player matches history DynamoDB table."
  value       = aws_dynamodb_table.player_matches_history.name
}

output "player_matches_history_table_arn" {
  description = "ARN of the player matches history DynamoDB table."
  value       = aws_dynamodb_table.player_matches_history.arn
}
