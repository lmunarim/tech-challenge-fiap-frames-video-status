resource "null_resource" "build_dotnet_lambda" {
  provisioner "local-exec" {
    command = <<EOT
        dotnet restore "${path.module}/src/Lambda/LambdaStatus/LambdaStatus.csproj" -t:rebuild 
        dotnet publish "${path.module}/src/Lambda/LambdaStatus/LambdaStatus.csproj" -t:rebuild -c Release -o "/home/runner/work/_temp/publish" 
    EOT
    interpreter = ["/bin/sh", "-c"]
  }
}

