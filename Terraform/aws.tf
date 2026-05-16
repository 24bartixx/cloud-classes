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

module "ecr" {
  source = "./aws_modules/ecr"

  repositories = [
    "tanks-clan-games",
    "tanks-clans",
    "tanks-inventories",
    "tanks-notifications",
    "tanks-player-matches-history",
    "tanks-rewards",
  ]
}

module "database" {
  source = "./aws_modules/database"

  name       = "tanks-terraform"
  vpc_id     = module.network.vpc_id
  subnet_ids = module.network.public_subnet_ids

  db_identifier   = "tanks-relational-db-terraform"
  db_name         = "TanksDb"
  master_username = "postgres"

  postgres_inbound_rules = [
    {
      name        = "home-zag-1"
      cidr_ipv4   = "109.197.45.119/32"
      description = "Allow PostgreSQL from home in Zagan"
    },
    # {
    #   name        = "university"
    #   cidr_ipv4   = "5.6.7.8/32"
    #   description = "Allow PostgreSQL from university IP"
    # },
  ]
}
