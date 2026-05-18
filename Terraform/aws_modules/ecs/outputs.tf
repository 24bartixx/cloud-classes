output "cluster_id" {
  description = "ID of the ECS cluster."
  value       = aws_ecs_cluster.this.id
}

output "cluster_name" {
  description = "Name of the ECS cluster."
  value       = aws_ecs_cluster.this.name
}

output "cluster_arn" {
  description = "ARN of the ECS cluster."
  value       = aws_ecs_cluster.this.arn
}

output "task_security_group_id" {
  description = "ID of the security group attached to ECS tasks."
  value       = aws_security_group.tasks.id
}

output "service_names" {
  description = "Names of the ECS services."
  value       = { for name, service in aws_ecs_service.service : name => service.name }
}

output "task_definition_arns" {
  description = "ARNs of the ECS task definitions."
  value       = { for name, task_definition in aws_ecs_task_definition.service : name => task_definition.arn }
}
