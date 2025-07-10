resource "null_resource" "build_dotnet_lambda" {
  provisioner "local-exec" {
    interpreter = ["/bin/bash", "-c"]
    command = <<-EOT
      dotnet restore "${path.module}/src/Lambda/LambdaStatus/LambdaStatus.csproj"
      dotnet publish "${path.module}/src/Lambda/LambdaStatus/LambdaStatus.csproj" -c Release -o "/home/runner/work/_temp/publish"
    EOT
  }
}