output "rds_postgres_security_group_id" {
  description = "ID of the PostgreSQL RDS security group."
  value       = aws_security_group.rds_postgres.id
}

output "rds_postgres_security_group_arn" {
  description = "ARN of the PostgreSQL RDS security group."
  value       = aws_security_group.rds_postgres.arn
}
