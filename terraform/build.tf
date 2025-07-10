resource "null_resource" "build_dotnet_lambda" {
  provisioner "local-exec" {
    interpreter = ["/bin/sh", "-c"]

    command = <<-EOT
      dotnet restore "./src/Lambda/LambdaStatus/LambdaStatus.csproj" -t:rebuild
      dotnet publish "./src/Lambda/LambdaStatus/LambdaStatus.csproj" -t:rebuild -c Release -o "/home/runner/work/_temp/publish"
    EOT
  }
}

