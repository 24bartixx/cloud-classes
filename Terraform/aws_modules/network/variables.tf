variable "name" {
  type        = string
  description = "Name prefix for the VPC and network resources."
}

variable "vpc_cidr" {
  type        = string
  description = "CIDR block for the VPC."
}

variable "availability_zone_count" {
  type        = number
  description = "Number of availability zones to use. Creates one public and one private subnet per availability zone."
  default     = 2

  validation {
    condition     = var.availability_zone_count >= 1 && var.availability_zone_count <= 3
    error_message = "availability_zone_count must be between 1 and 3."
  }
}
