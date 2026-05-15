variable "name" {
  description = "Name prefix used for database resources."
  type        = string
}

variable "vpc_id" {
  description = "ID of the VPC where the RDS security group should be created."
  type        = string
}

variable "postgres_inbound_rules" {
  description = "CIDR-based inbound rules that can connect to PostgreSQL on port 5432."
  type = list(object({
    name        = string
    cidr_ipv4   = string
    description = string
  }))
  default = []

  validation {
    condition = alltrue([
      for rule in var.postgres_inbound_rules : can(cidrhost(rule.cidr_ipv4, 0))
    ])
    error_message = "Each postgres_inbound_rules.cidr_ipv4 value must be a valid IPv4 CIDR block, for example 1.2.3.4/32."
  }
}
