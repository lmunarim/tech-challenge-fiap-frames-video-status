
# Variable for lambda zip path (provided by CI/CD)
variable "lambda_zip_path" {
  description = "Path to the Lambda deployment zip file"
  type        = string
  default     = "../lambda-deployment.zip"
}

resource "aws_lambda_function" "dotnet8_consumer" {
  function_name    = "lambda-status"
  handler          = "LambdaStatus::LambdaStatus.Function::FunctionHandler"
  runtime          = "dotnet8"
  role            = aws_iam_role.lambda_exec_role.arn
  filename        = var.lambda_zip_path
  source_code_hash = filebase64sha256(var.lambda_zip_path)
  timeout         = 30
  memory_size     = 512
}


# resource "aws_sqs_queue" "sqs-notifications" {
#   name                      = "tech-challenge-fiap-upload-notifications"
#   visibility_timeout_seconds = 30
#   message_retention_seconds = 86400
# }

resource "aws_iam_role" "lambda_exec_role" {
  name = "lambda-status-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [{
      Effect = "Allow",
      Principal = {
        Service = "lambda.amazonaws.com"
      },
      Action = "sts:AssumeRole"
    }]
  })
}

resource "aws_iam_policy" "lambda_sqs_policy" {
  name = "lambda-status-sqs-policy"
  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "sqs:ReceiveMessage",
          "sqs:DeleteMessage",
          "sqs:GetQueueAttributes"
        ],
        Resource = "*"
      },
      {
        Effect = "Allow",
        Action = [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents"
        ],
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_policy_attach" {
  role = aws_iam_role.lambda_exec_role.name
  policy_arn = aws_iam_policy.lambda_sqs_policy.arn
}

resource "aws_lambda_event_source_mapping" "sqs_trigger" {
  event_source_arn = "arn:aws:sqs:us-east-1:147997141255:tech-challenge-fiap-upload-notifications"
  function_name    = aws_lambda_function.dotnet8_consumer.arn
  batch_size       = 10
}