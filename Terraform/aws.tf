provider "aws" {
  region = "us-east-1"
  default_tags {
    tags = {
      Project = "terraform-tanks"
    }
  }
}

module "network" {
  source = "./aws_modules/network"

  name                    = "tanks-terraform"
  vpc_cidr                = "10.0.0.0/16"
  availability_zone_count = 2
}

module "storage" {
  source = "./aws_modules/storage"

  s3_bucket_prefix = "pwr-ist-280462-tanks-terraform"
}

module "dynamodb" {
  source = "./aws_modules/dynamodb"

  inventories_table_name            = "tanks-inventories-terraform"
  player_matches_history_table_name = "tanks-player-matches-history-terraform"
}
