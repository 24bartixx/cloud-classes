resource "aws_security_group" "rds_postgres" {
  name        = "tanks-relational-db-terraform"
  description = "Security group for PostgreSQL RDS."
  vpc_id      = var.vpc_id

  tags = {
    Name = "tanks-relational-db-terraform"
  }
}

resource "aws_vpc_security_group_ingress_rule" "postgres" {
  for_each = {
    for rule in var.postgres_inbound_rules : rule.name => rule
  }

  security_group_id = aws_security_group.rds_postgres.id

  cidr_ipv4   = each.value.cidr_ipv4
  ip_protocol = "tcp"
  from_port   = 5432
  to_port     = 5432
  description = each.value.description
}

resource "aws_vpc_security_group_egress_rule" "all_outbound" {
  security_group_id = aws_security_group.rds_postgres.id

  cidr_ipv4   = "0.0.0.0/0"
  ip_protocol = "-1"
  description = "Allow all outbound traffic."
}
