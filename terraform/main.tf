resource "aws_sqs_queue" "sqs-notifications" {
  name                      = "tech-challenge-fiap-upload-notifications"
  visibility_timeout_seconds = 30
}

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
  role       = aws_iam_role.lambda_exec_role.name
  policy_arn = aws_iam_policy.lambda_sqs_policy.arn
}

data "archive_file" "lambda" {
  type        = "zip"
  source_dir  = "/home/runner/work/_temp/publish/"
  output_path = "lambda.zip"
  depends_on  = [null_resource.build_dotnet_lambda]
}

resource "aws_lambda_function" "dotnet8_consumer" {
  depends_on    = [data.archive_file.lambda]
  function_name = "lambda-status"
  filename      = "publish/lambda.zip" # Path to your zipped .NET 8 Lambda
  handler       = "LambdaStatus::LambdaStatus.Function::FunctionHandler"
  runtime       = "dotnet8"
  role          = aws_iam_role.lambda_exec_role.arn
  source_code_hash = filebase64sha256("publish/lambda.zip")
}

resource "aws_lambda_event_source_mapping" "sqs_trigger" {
  event_source_arn = aws_sqs_queue.sqs-notifications.arn
  function_name    = aws_lambda_function.dotnet8_consumer.arn
  batch_size       = 10
}

# -------------------------------------------------------------------------------

# ## Archiving the Artifacts
# data "archive_file" "lambda" {
#   type        = "zip"
#   source_dir  = "/home/runner/work/_temp/publish/"
#   output_path = "lambda.zip"
#   depends_on  = [null_resource.build_dotnet_lambda]
# }

# resource "aws_lambda_function" "lambda" {
#   depends_on       = [data.archive_file.lambda]
#   filename         = "lambda.zip"
#   function_name    = "lambda-status"
#   role             = aws_iam_role.lambda.arn
#   handler          = "LambdaStatus::LambdaStatus.LambdaHandler::handleRequest" #Class is build from a source generator
#   source_code_hash = data.archive_file.lambda.output_base64sha256 # ?
#   runtime          = "dotnet8"
#   architectures    = ["x86_64"]
#   memory_size      = "512"
#   timeout          = 10
# }

# resource "aws_iam_role" "lambda" {
#   name                = "lambda-status-role"
#   assume_role_policy  = data.aws_iam_policy_document.assume_role_policy.json
# }

# data "aws_iam_policy_document" "assume_role_policy" {
#   version = "2012-10-17"
#   statement {
#     actions = [
#       "sts:AssumeRole"
#     ]
#     principals {
#       type        = "Service"
#       identifiers = [
#         "lambda.amazonaws.com"
#         ]
#     }
#   }
# }

# # Lambda execution

# data "aws_iam_policy" "lambdabasic" {
#   arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
# }

# resource "aws_iam_role_policy_attachment" "lambdabasic" {
#   role       = aws_iam_role.lambda.name
#   policy_arn = data.aws_iam_policy.lambdabasic.arn
# }

# resource "aws_apigatewayv2_api" "lambda" {
#   name          = "gtw-usuario"
#   protocol_type = "HTTP"
# }

# resource "aws_apigatewayv2_stage" "lambda" {
#   api_id = aws_apigatewayv2_api.lambda.id
#   name        = "gtw-usuario"
#   auto_deploy = true
# }

# resource "aws_apigatewayv2_integration" "lambda" {
#   api_id = aws_apigatewayv2_api.lambda.id
#   integration_uri    = aws_lambda_function.lambda.invoke_arn
#   integration_type   = "AWS_PROXY"
#   integration_method = "POST"
# }

# resource "aws_apigatewayv2_route" "lambda" {
#   api_id = aws_apigatewayv2_api.lambda.id
#   route_key = "POST /usuario/validar"
#   target    = "integrations/${aws_apigatewayv2_integration.lambda.id}"
# }

# resource "aws_lambda_permission" "api_gw" {
#   statement_id  = "AllowExecutionFromAPIGateway"
#   action        = "lambda:InvokeFunction"
#   function_name = aws_lambda_function.lambda.function_name
#   principal     = "apigateway.amazonaws.com"
#   source_arn = "${aws_apigatewayv2_api.lambda.execution_arn}/*/*"
# }