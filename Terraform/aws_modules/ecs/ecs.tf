data "aws_region" "current" {}

locals {
  environment = {
    for service_name, service in var.services :
    service_name => [
      for key, value in service.environment : {
        name  = key
        value = value
      }
    ]
  }

  ingress_rules = {
    for pair in setproduct(distinct([for service in values(var.services) : service.container_port]), var.allowed_ingress_cidr_blocks) :
    "${pair[0]}-${replace(pair[1], "/", "_")}" => {
      port       = pair[0]
      cidr_block = pair[1]
    }
  }
}

resource "aws_ecs_cluster" "this" {
  name = var.cluster_name

  tags = {
    Name = var.cluster_name
  }
}

resource "aws_cloudwatch_log_group" "service" {
  for_each = var.services

  name              = "/ecs/${each.key}"
  retention_in_days = 7

  tags = {
    Name = "/ecs/${each.key}"
  }
}

resource "aws_security_group" "tasks" {
  name        = "${var.cluster_name}-tasks"
  description = "Security group for ECS Fargate tasks."
  vpc_id      = var.vpc_id

  tags = {
    Name = "${var.cluster_name}-tasks"
  }
}

resource "aws_vpc_security_group_ingress_rule" "container" {
  for_each = local.ingress_rules

  security_group_id = aws_security_group.tasks.id

  cidr_ipv4   = each.value.cidr_block
  ip_protocol = "tcp"
  from_port   = each.value.port
  to_port     = each.value.port
  description = "Allow inbound traffic to ECS tasks."
}

resource "aws_vpc_security_group_egress_rule" "all_outbound" {
  security_group_id = aws_security_group.tasks.id

  cidr_ipv4   = "0.0.0.0/0"
  ip_protocol = "-1"
  description = "Allow all outbound traffic."
}

resource "aws_ecs_task_definition" "service" {
  for_each = var.services

  family                   = each.key
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = each.value.cpu
  memory                   = each.value.memory
  execution_role_arn       = var.execution_role_arn
  task_role_arn            = var.task_role_arn

  container_definitions = jsonencode([
    {
      name      = each.key
      image     = each.value.image
      essential = true

      portMappings = [
        {
          containerPort = each.value.container_port
          hostPort      = each.value.container_port
          protocol      = "tcp"
        }
      ]

      environment = local.environment[each.key]

      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = aws_cloudwatch_log_group.service[each.key].name
          awslogs-region        = data.aws_region.current.region
          awslogs-stream-prefix = "ecs"
        }
      }
    }
  ])

  tags = {
    Name = each.key
  }
}

resource "aws_ecs_service" "service" {
  for_each = var.services

  name            = each.key
  cluster         = aws_ecs_cluster.this.id
  task_definition = aws_ecs_task_definition.service[each.key].arn
  desired_count   = each.value.desired_count
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = var.subnet_ids
    security_groups  = [aws_security_group.tasks.id]
    assign_public_ip = var.assign_public_ip
  }

  tags = {
    Name = each.key
  }
}
