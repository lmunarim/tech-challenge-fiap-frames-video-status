output "lambda_function_name" {
  description = "Name of the Lambda function"
  value       = aws_lambda_function.dotnet8_consumer.function_name
}

output "lambda_function_arn" {
  description = "ARN of the Lambda function"
  value       = aws_lambda_function.dotnet8_consumer.arn
}

# output "sqs_queue_name" {
#   description = "Name of the SQS queue"
#   value       = aws_sqs_queue.sqs-notifications.name
# }

# output "sqs_queue_url" {
#   description = "URL of the SQS queue"
#   value       = aws_sqs_queue.sqs-notifications.url
# }
