# AWS Lambda Deployment Guide

Este projeto utiliza GitHub Actions para automatizar o build e deploy da função Lambda no AWS.

## Configuração do CI/CD

### 1. Configurar Secrets no GitHub

No seu repositório GitHub, vá em `Settings > Secrets and variables > Actions` e adicione:

- `AWS_ACCESS_KEY_ID`: Sua chave de acesso AWS
- `AWS_SECRET_ACCESS_KEY`: Sua chave secreta AWS

### 2. Configurar IAM User no AWS

Crie um usuário IAM com as seguintes permissões:

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "lambda:*",
                "iam:*",
                "sqs:*",
                "logs:*"
            ],
            "Resource": "*"
        }
    ]
}
```

## Como Funciona o Pipeline

### Trigger
- Push para `main` ou `develop`: Deploy automático
- Pull Request: Apenas build e validação

### Etapas
1. **Build**: Compila o projeto .NET 8
2. **Optimize**: Remove arquivos desnecessários para reduzir o tamanho
3. **Package**: Cria um zip otimizado
4. **Deploy**: Usa Terraform para criar/atualizar a infraestrutura

## Otimizações Implementadas

Para resolver o erro de "Request Entity Too Large":

1. **Remoção de arquivos desnecessários**:
   - Arquivos `.pdb` (debug symbols)
   - Arquivos `.xml` (documentação)
   - Runtime packages para Windows/macOS (mantém apenas Linux)

2. **Build otimizado**:
   - `--self-contained false`: Usa runtime compartilhado
   - `-r linux-x64`: Target específico para Lambda

3. **Compressão máxima**: Usa `CompressionLevel.Optimal`

## Deploy Local

Para testar localmente, execute:

```powershell
cd terraform
.\deploy-local.ps1
```

## Estrutura dos Arquivos

```
├── .github/workflows/
│   └── deploy-lambda.yml      # Pipeline de CI/CD
├── terraform/
│   ├── main.tf               # Recursos AWS
│   ├── provider.tf           # Configuração do Terraform
│   ├── deploy-local.ps1      # Script para deploy local
│   └── .gitignore           # Arquivos ignorados
└── src/Lambda/              # Código da função Lambda
```

## Monitoramento

Após o deploy, você pode:

1. Ver os logs no CloudWatch: `/aws/lambda/lambda-status-[suffix]`
2. Testar a função no AWS Console
3. Monitorar métricas no CloudWatch

## Troubleshooting

### Erro: Package muito grande
- Verifique se todas as otimizações estão sendo aplicadas
- Considere usar AWS Lambda Layers para dependências grandes

### Erro: Permissions
- Verifique se as credenciais AWS têm as permissões necessárias
- Confirme se a região está correta

### Erro: Terraform State
- Se necessário, execute `terraform init` localmente
- Para reiniciar: `rm -rf .terraform/ terraform.tfstate*`
