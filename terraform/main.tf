data "archive_file" "lambda_zip" {
  type        = "zip"
  source_dir  = "${path.module}/publish"
  output_path = "${path.module}/publish/lambda.zip"
}

resource "aws_lambda_function" "dotnet8_consumer" {
  depends_on    = [data.archive_file.lambda_zip]
  function_name = "lambda-status"
  handler       = "LambdaStatus::LambdaStatus.Function::FunctionHandler"
  runtime       = "dotnet8"
  role          = aws_iam_role.lambda_exec_role.arn
  filename         = "${path.module}/lambda.zip"
  source_code_hash = filebase64sha256("${path.module}/lambda.zip")
  # filename         = data.archive_file.lambda_zip.output_path
  # source_code_hash = data.archive_file.lambda_zip.output_base64sha256
}


resource "aws_sqs_queue" "sqs-notifications" {
  name                      = "tech-challenge-fiap-upload-notifications"
  visibility_timeout_seconds = 30
  message_retention_seconds = 86400
}

data "aws_iam_role" "lambda_exec_role" {
  name = "lambda-status-role"
}

resource "aws_iam_role" "lambda_exec_role" {
  name = aws_iam_role.lambda_exec_role.name
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
        Resource = aws_sqs_queue.sqs-notifications.arn
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
  #role       = aws_iam_role.lambda_exec_role.name
  role       = data.aws_iam_role.lambda_exec_role.name
  policy_arn = aws_iam_policy.lambda_sqs_policy.arn
}

resource "aws_lambda_event_source_mapping" "sqs_trigger" {
  event_source_arn = aws_sqs_queue.sqs-notifications.arn
  function_name    = aws_lambda_function.dotnet8_consumer.arn
  batch_size       = 10
}