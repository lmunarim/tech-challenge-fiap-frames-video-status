resource "null_resource" "build_dotnet_lambda" {
  provisioner "local-exec" {
    interpreter = ["/bin/bash", "-c"]
    command = <<-EOT
      dotnet restore "./src/Lambda/LambdaStatus/LambdaStatus.csproj"
      dotnet publish "./src/Lambda/LambdaStatus/LambdaStatus.csproj" -c Release -o "/home/runner/work/_temp/publish"
    EOT
  }
}