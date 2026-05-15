variable "inventories_table_name" {
  description = "Name of the DynamoDB table used by the inventories service."
  type        = string
}

variable "player_matches_history_table_name" {
  description = "Name of the DynamoDB table used by the player matches history service."
  type        = string
}
