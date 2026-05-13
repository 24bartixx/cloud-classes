data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
  clan_wars_registry_bucket_name = "${var.s3_bucket_prefix}-${data.aws_caller_identity.current.account_id}-${data.aws_region.current.region}-an"
}

resource "aws_s3_bucket" "clan_wars_registry" {
  bucket           = local.clan_wars_registry_bucket_name
  bucket_namespace = "account-regional"
}

resource "aws_s3_bucket_public_access_block" "clan_wars_registry" {
  bucket = aws_s3_bucket.clan_wars_registry.id

  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}
