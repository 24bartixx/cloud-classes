provider "aws" {
  region = "us-east-1"
  default_tags {
    tags = {
      Project = "terraform-tanks"
    }
  }
}

module "storage" {
  source = "./aws_modules/storage"
  s3_bucket_prefix = "pwr-ist-280462-tanks-terraform"
}

# Create a VPC
# resource "aws_vpc" "example" {
#   cidr_block = "10.0.0.0/16"
# }
