terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.67.0"
    }
    null = {
      source  = "hashicorp/null"
      version = "~> 3.2.0"
    }
  }
}

provider "aws" {
  region                      = "us-east-1"
  access_key                  = "test"
  secret_key                  = "test"
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true
  s3_use_path_style           = true
  
  endpoints {
    s3     = "http://127.0.0.1:4566"
    lambda = "http://127.0.0.1:4566"
    iam    = "http://127.0.0.1:4566"
    ssm    = "http://127.0.0.1:4566"
    sts    = "http://127.0.0.1:4566"
  }
}

# 5.a Два S3 бакети
resource "aws_s3_bucket" "start_bucket" {
  bucket = "s3-start"
}

resource "aws_s3_bucket" "finish_bucket" {
  bucket = "s3-finish"
}

# 5.b S3 lifecycle policy (DevOps-хак для LocalStack)
resource "null_resource" "finish_lifecycle_hack" {
  depends_on = [aws_s3_bucket.finish_bucket]

  provisioner "local-exec" {
    command = "aws --endpoint-url=http://localhost:4566 s3api put-bucket-lifecycle-configuration --bucket ${aws_s3_bucket.finish_bucket.id} --lifecycle-configuration '{\"Rules\":[{\"ID\":\"cleanup-old-files\",\"Status\":\"Enabled\",\"Filter\":{\"Prefix\":\"\"},\"Expiration\":{\"Days\":7}}]}'"
  }
}

# 5.e Додатковий сервіс: SSM Parameter Store
resource "aws_ssm_parameter" "target_bucket_param" {
  name  = "/lab/s3/target_bucket"
  type  = "String"
  value = aws_s3_bucket.finish_bucket.bucket
}

data "archive_file" "lambda_zip" {
  type        = "zip"
  source_dir  = "${path.module}/lambda"
  output_path = "${path.module}/lambda.zip"
}

# 5.c Lambda функція
resource "aws_lambda_function" "copy_lambda" {
  filename      = data.archive_file.lambda_zip.output_path
  function_name = "s3-copy-function"
  role          = aws_iam_role.lambda_role.arn
  handler       = "lambda_function.lambda_handler"
  runtime       = "python3.9"

  environment {
    variables = {
      SSM_PARAM_NAME = aws_ssm_parameter.target_bucket_param.name
    }
  }
}

resource "aws_iam_role" "lambda_role" {
  name = "lambda_execution_role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = { Service = "lambda.amazonaws.com" }
    }]
  })
}

# Дозвіл для S3 тригерити Lambda
resource "aws_lambda_permission" "allow_s3" {
  statement_id  = "AllowExecutionFromS3Bucket"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.copy_lambda.function_name
  principal     = "s3.amazonaws.com"
  source_arn    = aws_s3_bucket.start_bucket.arn
}

# 5.d S3-bucket event: Тригер для Lambda
resource "aws_s3_bucket_notification" "bucket_notification" {
  bucket = aws_s3_bucket.start_bucket.id
  lambda_function {
    lambda_function_arn = aws_lambda_function.copy_lambda.arn
    events              = ["s3:ObjectCreated:*"]
  }
  depends_on = [aws_lambda_permission.allow_s3]
}
