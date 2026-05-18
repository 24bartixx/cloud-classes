provider "aws" {
  region = "us-east-1"
  default_tags {
    tags = {
      Project = "terraform-tanks"
    }
  }
}

data "aws_iam_role" "lab_role" {
  name = "LabRole"
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
    {
      name        = "home-wro-1"
      cidr_ipv4   = "89.151.17.193/32"
      description = "Allow PostgreSQL from home in Zagan"
    },
    
    # {
    #   name        = "university"
    #   cidr_ipv4   = "5.6.7.8/32"
    #   description = "Allow PostgreSQL from university IP"
    # },
  ]
}

module "ecs" {
  source = "./aws_modules/ecs"

  cluster_name       = "tanks-cluster"
  vpc_id             = module.network.vpc_id
  subnet_ids         = module.network.public_subnet_ids
  task_role_arn      = data.aws_iam_role.lab_role.arn
  execution_role_arn = data.aws_iam_role.lab_role.arn
  assign_public_ip   = true

  allowed_ingress_cidr_blocks = ["109.197.45.119/32", "89.151.17.193/32"]

  services = {
    tanks-clan-games = {
      image          = "${module.ecr.repository_urls["tanks-clan-games"]}:latest"
      container_port = 8080
      cpu            = 256
      memory         = 512
      desired_count  = 1
      environment = {
        ASPNETCORE_HTTP_PORTS   = "8080"
        AWS_REGION              = "us-east-1"
        AWS_DEFAULT_REGION      = "us-east-1"
        RabbitMq__ConnectionUrl = var.rabbitmq_connection_url
      }
    }

    tanks-clans = {
      image          = "${module.ecr.repository_urls["tanks-clans"]}:latest"
      container_port = 8080
      cpu            = 256
      memory         = 512
      desired_count  = 1
      environment = {
        ASPNETCORE_HTTP_PORTS                = "8080"
        AWS_REGION                           = "us-east-1"
        AWS_DEFAULT_REGION                   = "us-east-1"
        AWS__Region                          = "us-east-1"
        RabbitMq__ConnectionUrl              = var.rabbitmq_connection_url
        ConnectionStrings__DefaultConnection = var.clans_postgres_connection_string
        S3__BucketName                       = module.storage.clan_wars_registry_bucket_name
      }
    }

    tanks-inventories = {
      image          = "${module.ecr.repository_urls["tanks-inventories"]}:latest"
      container_port = 8080
      cpu            = 256
      memory         = 512
      desired_count  = 1
      environment = {
        ASPNETCORE_HTTP_PORTS   = "8080"
        AWS_REGION              = "us-east-1"
        AWS_DEFAULT_REGION      = "us-east-1"
        AWS__Region             = "us-east-1"
        RabbitMq__ConnectionUrl = var.rabbitmq_connection_url
        DynamoDb__TableName     = "tanks-inventories-terraform"
      }
    }

    tanks-notifications = {
      image          = "${module.ecr.repository_urls["tanks-notifications"]}:latest"
      container_port = 8080
      cpu            = 256
      memory         = 512
      desired_count  = 1
      environment = {
        ASPNETCORE_HTTP_PORTS                = "8080"
        AWS_REGION                           = "us-east-1"
        AWS_DEFAULT_REGION                   = "us-east-1"
        RabbitMq__ConnectionUrl              = var.rabbitmq_connection_url
        ConnectionStrings__DefaultConnection = var.notifications_postgres_connection_string
      }
    }

    tanks-player-matches-history = {
      image          = "${module.ecr.repository_urls["tanks-player-matches-history"]}:latest"
      container_port = 8080
      cpu            = 256
      memory         = 512
      desired_count  = 1
      environment = {
        ASPNETCORE_HTTP_PORTS   = "8080"
        AWS_REGION              = "us-east-1"
        AWS_DEFAULT_REGION      = "us-east-1"
        AWS__Region             = "us-east-1"
        RabbitMq__ConnectionUrl = var.rabbitmq_connection_url
        DynamoDb__TableName     = "tanks-player-matches-history-terraform"
      }
    }

    tanks-rewards = {
      image          = "${module.ecr.repository_urls["tanks-rewards"]}:latest"
      container_port = 8080
      cpu            = 256
      memory         = 512
      desired_count  = 1
      environment = {
        ASPNETCORE_HTTP_PORTS                = "8080"
        AWS_REGION                           = "us-east-1"
        AWS_DEFAULT_REGION                   = "us-east-1"
        RabbitMq__ConnectionUrl              = var.rabbitmq_connection_url
        ConnectionStrings__DefaultConnection = var.rewards_postgres_connection_string
      }
    }
  }
}

resource "aws_vpc_security_group_ingress_rule" "postgres_from_ecs" {
  security_group_id            = module.database.rds_postgres_security_group_id
  referenced_security_group_id = module.ecs.task_security_group_id

  ip_protocol = "tcp"
  from_port   = 5432
  to_port     = 5432
  description = "Allow PostgreSQL from ECS tasks."
}
