variable "cluster_name" {
  description = "Name of the ECS cluster."
  type        = string
}

variable "vpc_id" {
  description = "ID of the VPC where ECS services run."
  type        = string
}

variable "subnet_ids" {
  description = "Subnet IDs where ECS services run."
  type        = list(string)
}

variable "task_role_arn" {
  description = "IAM role ARN used as ECS task role."
  type        = string
}

variable "execution_role_arn" {
  description = "IAM role ARN used as ECS execution role."
  type        = string
}

variable "services" {
  description = "ECS services and task definitions to create."
  type = map(object({
    image          = string
    container_port = number
    cpu            = number
    memory         = number
    desired_count  = number
    environment    = map(string)
  }))
}

variable "assign_public_ip" {
  description = "Whether ECS tasks should receive public IP addresses."
  type        = bool
  default     = true
}

variable "allowed_ingress_cidr_blocks" {
  description = "CIDR blocks allowed to reach service containers on their container port."
  type        = list(string)
  default     = ["0.0.0.0/0"]
}
