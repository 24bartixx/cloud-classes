output "rds_postgres_security_group_id" {
  description = "ID of the PostgreSQL RDS security group."
  value       = aws_security_group.rds_postgres.id
}

output "rds_postgres_security_group_arn" {
  description = "ARN of the PostgreSQL RDS security group."
  value       = aws_security_group.rds_postgres.arn
}

output "rds_postgres_instance_id" {
  description = "ID of the PostgreSQL RDS instance."
  value       = aws_db_instance.postgres.id
}

output "rds_postgres_instance_arn" {
  description = "ARN of the PostgreSQL RDS instance."
  value       = aws_db_instance.postgres.arn
}

output "rds_postgres_endpoint" {
  description = "Connection endpoint of the PostgreSQL RDS instance."
  value       = aws_db_instance.postgres.endpoint
}

output "rds_postgres_address" {
  description = "Hostname of the PostgreSQL RDS instance."
  value       = aws_db_instance.postgres.address
}

output "rds_postgres_port" {
  description = "Port of the PostgreSQL RDS instance."
  value       = aws_db_instance.postgres.port
}

output "rds_postgres_master_user_secret_arn" {
  description = "ARN of the AWS-managed Secrets Manager secret for the PostgreSQL master user."
  value       = aws_db_instance.postgres.master_user_secret[0].secret_arn
}
