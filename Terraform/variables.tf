variable "rabbitmq_connection_url" {
  description = "RabbitMQ connection URL used by all services."
  type        = string
  sensitive   = true
}

variable "clans_postgres_connection_string" {
  description = "PostgreSQL connection string used by the Clans service."
  type        = string
  sensitive   = true
}

variable "notifications_postgres_connection_string" {
  description = "PostgreSQL connection string used by the Notifications service."
  type        = string
  sensitive   = true
}

variable "rewards_postgres_connection_string" {
  description = "PostgreSQL connection string used by the Rewards service."
  type        = string
  sensitive   = true
}
