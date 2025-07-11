# Tech Challenge FIAP - Frames Video Status

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![AWS Lambda](https://img.shields.io/badge/AWS-Lambda-orange.svg)](https://aws.amazon.com/lambda/)
[![Terraform](https://img.shields.io/badge/Infrastructure-Terraform-purple.svg)](https://terraform.io/)
[![DynamoDB](https://img.shields.io/badge/Database-DynamoDB-yellow.svg)](https://aws.amazon.com/dynamodb/)

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Funcionalidades](#funcionalidades)
- [Infraestrutura](#infraestrutura)
- [Configuração e Deploy](#configuração-e-deploy)
- [Desenvolvimento](#desenvolvimento)
- [Testes](#testes)
- [Contribuição](#contribuição)

## 🎯 Sobre o Projeto

O **Tech Challenge FIAP - Frames Video Status** é uma aplicação serverless desenvolvida para o curso de Pós-graduação em Software Architecture da FIAP. O sistema é responsável por processar atualizações de status de upload de vídeos e notificar usuários quando ocorrem problemas durante o processamento.

A aplicação funciona como um consumer de mensagens SQS que processa informações sobre uploads de vídeo, atualiza o status no DynamoDB e envia notificações por e-mail quando necessário.

## 🏗️ Arquitetura

A solução segue os princípios de **Clean Architecture** e **Domain Driven Design (DDD)**, organizando o código em camadas bem definidas:

```
┌─────────────────────────────────────────────────────────┐
│                    AWS SQS Queue                       │
│          tech-challenge-fiap-upload-notifications      │
└─────────────────┬───────────────────────────────────────┘
                  │
                  │ Event Trigger
                  ▼
┌─────────────────────────────────────────────────────────┐
│                AWS Lambda Function                     │
│                lambda-status-not                       │
├─────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────────────────┐   │
│  │   Application   │  │         Domain              │   │
│  │   Use Cases     │  │       Entities              │   │
│  │                 │  │      Interfaces             │   │
│  └─────────────────┘  └─────────────────────────────┘   │
│                                                         │
│  ┌─────────────────┐  ┌─────────────────────────────┐   │
│  │ Infrastructure  │  │        Services             │   │
│  │  Repositories   │  │    Secret Manager           │   │
│  │                 │  │       SMTP                  │   │
│  └─────────────────┘  └─────────────────────────────┘   │
└─────────────────┬───────────────────┬───────────────────┘
                  │                   │
                  ▼                   ▼
    ┌─────────────────────┐  ┌─────────────────────┐
    │    AWS DynamoDB     │  │   AWS Secrets       │
    │   Video Uploads     │  │     Manager         │
    │     Database        │  │   SMTP Config       │
    └─────────────────────┘  └─────────────────────┘
```

### Fluxo de Processamento

1. **Mensagem SQS**: Uma mensagem contendo dados de upload de vídeo é recebida na fila SQS
2. **Lambda Trigger**: A função Lambda é automaticamente disparada para processar a mensagem
3. **Processamento**: O sistema verifica o status do upload e executa as ações necessárias:
   - Se o status for `ErroProcessamento`: envia notificação por e-mail
   - Caso contrário: atualiza ou insere o registro no DynamoDB
4. **Persistência**: Os dados são salvos no DynamoDB
5. **Notificação**: Em caso de erro, um e-mail é enviado ao usuário

## 🛠️ Tecnologias Utilizadas

### Backend
- **.NET 8.0**: Framework principal da aplicação
- **AWS Lambda**: Plataforma serverless para execução do código
- **AWS SDK**: Integração com serviços AWS (DynamoDB, SQS, Secrets Manager)
- **System.Text.Json**: Serialização/deserialização JSON
- **Microsoft.Extensions.DependencyInjection**: Injeção de dependência

### Infraestrutura
- **AWS SQS**: Fila de mensagens para comunicação assíncrona
- **AWS DynamoDB**: Banco de dados NoSQL para persistência
- **AWS Secrets Manager**: Gerenciamento seguro de credenciais
- **AWS IAM**: Controle de acesso e permissões
- **Terraform**: Infrastructure as Code (IaC)

### Ferramentas de Desenvolvimento
- **xUnit**: Framework de testes unitários
- **AWS Lambda Tools**: Ferramentas para desenvolvimento e deploy

## 📁 Estrutura do Projeto

```
tech-challenge-fiap-frames-video-status/
├── src/
│   ├── Core/                           # Camada de domínio
│   │   ├── fiap.Application/           # Casos de uso e interfaces
│   │   │   ├── Interfaces/
│   │   │   │   ├── IEmailApplication.cs
│   │   │   │   └── IVideoUploadApplication.cs
│   │   │   └── UseCases/
│   │   │       ├── EmailApplication.cs
│   │   │       └── VideoUploadApplication.cs
│   │   └── fiap.Domain/                # Entidades e regras de negócio
│   │       ├── Entities/
│   │       │   ├── SecretEmail.cs
│   │       │   ├── StatusUpload.cs
│   │       │   ├── Usuario.cs
│   │       │   └── VideoUpload.cs
│   │       └── Interfaces/
│   │           ├── ISecretManagerService.cs
│   │           └── IVideoUploadRepository.cs
│   ├── Infrastructure/                 # Camada de infraestrutura
│   │   ├── Data/
│   │   │   └── fiap.Repositories/      # Repositórios de dados
│   │   │       └── VideoUploadRepository.cs
│   │   └── Services/
│   │       └── fiap.Services/          # Serviços externos
│   │           └── SecretManagerService.cs
│   ├── Lambda/                         # Função Lambda
│   │   ├── Function.cs                 # Entry point da Lambda
│   │   ├── LambdaStatus.csproj        # Configurações do projeto
│   │   └── aws-lambda-tools-defaults.json
│   └── Tests/                          # Testes unitários
│       └── fiap.Tests/
│           ├── Application/
│           ├── Repositories/
│           └── Service/
├── terraform/                          # Infraestrutura como código
│   ├── main.tf                        # Recursos principais
│   ├── provider.tf                    # Configuração do provider AWS
│   ├── build.tf                       # Scripts de build
│   └── outputs.tf                     # Outputs do Terraform
└── docs/                              # Documentação
    ├── API.md
    ├── ARCHITECTURE.md
    ├── DEPLOYMENT.md
    └── DEVELOPMENT.md
```

## ⚙️ Funcionalidades

### 🔄 Processamento de Status de Upload

A aplicação processa diferentes status de upload de vídeo:

- **Enviado** (1): Vídeo foi enviado para processamento
- **Processando** (2): Vídeo está sendo processado
- **Finalizado** (3): Processamento concluído com sucesso
- **Cancelado** (4): Processamento foi cancelado
- **ErroProcessamento** (5): Erro durante o processamento
- **SalvandoS3** (6): Salvando arquivo no S3
- **ArquivoSalvoS3** (7): Arquivo salvo com sucesso no S3

### 📧 Sistema de Notificações

- **Notificação por E-mail**: Envio automático de e-mails quando ocorrem erros de processamento
- **Configuração Segura**: Credenciais SMTP armazenadas no AWS Secrets Manager
- **Templates Personalizados**: E-mails com informações detalhadas sobre o status

### 💾 Persistência de Dados

- **DynamoDB**: Armazenamento de informações de upload
- **Operações CRUD**: Inserção, consulta e atualização de registros
- **Chave Primária**: Identificação única por ID do upload

## 🏭 Infraestrutura

A infraestrutura é gerenciada através do **Terraform** e inclui os seguintes recursos AWS:

### AWS Lambda Function
```hcl
resource "aws_lambda_function" "dotnet8_consumer" {
  function_name    = "lambda-status-not"
  handler          = "LambdaStatus::LambdaStatus.Function::FunctionHandler"
  runtime          = "dotnet8"
  timeout          = 30
  memory_size      = 512
}
```

### IAM Roles e Policies
- **Role de Execução**: Permissões para a Lambda acessar SQS, DynamoDB e CloudWatch
- **Políticas de Segurança**: Acesso restrito apenas aos recursos necessários

### Event Source Mapping
- **Trigger SQS**: Configuração automática para processar mensagens da fila
- **Batch Size**: Processamento de até 10 mensagens por invocação

### Recursos Necessários
- **SQS Queue**: `tech-challenge-fiap-upload-notifications`
- **DynamoDB Table**: Para persistência dos dados de upload
- **Secrets Manager**: Armazenamento das credenciais SMTP

## 🚀 Configuração e Deploy

### Pré-requisitos

- .NET 8.0 SDK
- AWS CLI configurado
- Terraform >= 1.0
- Conta AWS com permissões adequadas

### Deploy da Infraestrutura

1. **Configurar variáveis do Terraform**:
```bash
cd terraform/
```

2. **Inicializar Terraform**:
```bash
terraform init
```

3. **Planejar deployment**:
```bash
terraform plan
```

4. **Aplicar infraestrutura**:
```bash
terraform apply
```

### Build e Deploy da Aplicação

1. **Restaurar dependências**:
```bash
dotnet restore src/Lambda/LambdaStatus.csproj
```

2. **Publicar aplicação**:
```bash
dotnet publish src/Lambda/LambdaStatus.csproj -c Release -o ./publish
```

3. **Criar pacote ZIP**:
```bash
cd publish && zip -r ../lambda-deployment.zip .
```

4. **Atualizar Lambda** (via Terraform ou AWS CLI):
```bash
aws lambda update-function-code --function-name lambda-status-not --zip-file fileb://lambda-deployment.zip
```

### Configuração de Secrets

Configurar as credenciais SMTP no AWS Secrets Manager:

```json
{
  "Smtp": "smtp.gmail.com",
  "User": "seu-email@gmail.com",
  "Pass": "sua-senha-app",
  "MailAdress": "seu-email@gmail.com"
}
```

## 💻 Desenvolvimento

### Executando Localmente

1. **Configurar AWS credentials**:
```bash
aws configure
```

2. **Restaurar dependências**:
```bash
dotnet restore
```

3. **Executar testes**:
```bash
dotnet test
```

### Estrutura de Desenvolvimento

O projeto segue os princípios de **Clean Architecture**:

- **Domain**: Entidades e regras de negócio
- **Application**: Casos de uso e interfaces
- **Infrastructure**: Implementações de repositórios e serviços
- **Lambda**: Entry point e configuração de DI

### Injeção de Dependência

```csharp
private static void ConfigureServices(IServiceCollection serviceCollection)
{
    serviceCollection.AddLogging();
    serviceCollection.AddAWSService<IAmazonDynamoDB>();
    serviceCollection.AddTransient<IVideoUploadApplication, VideoUploadApplication>();
    serviceCollection.AddTransient<IEmailApplication, EmailApplication>();
    serviceCollection.AddSingleton<IAmazonSecretsManager>(new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName("us-east-1")));
    serviceCollection.AddSingleton<ISecretManagerService, SecretManagerService>();
    serviceCollection.AddTransient<IVideoUploadRepository, VideoUploadRepository>();
}
```

## 🧪 Testes

O projeto inclui testes unitários para todas as camadas:

### Executar Testes
```bash
dotnet test src/Tests/fiap.Tests/
```

### Cobertura de Testes
- **Application Layer**: Testes dos casos de uso
- **Repository Layer**: Testes dos repositórios
- **Service Layer**: Testes dos serviços externos

### Estrutura de Testes
```
Tests/
├── Application/
│   ├── EmailApplicationTests.cs
│   └── VideoUploadApplicationTests.cs
├── Repositories/
│   └── VideoUploadRepositoryTests.cs
└── Service/
    └── SecretManagerServiceTests.cs
```

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Padrões de Código

- Seguir convenções do C# e .NET
- Manter cobertura de testes acima de 80%
- Documentar métodos públicos
- Seguir princípios SOLID

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👥 Equipe

Desenvolvido como parte do Tech Challenge da FIAP - Pós-graduação em Software Architecture.

## 📞 Suporte

Para dúvidas ou suporte, entre em contato através dos canais oficiais da FIAP ou abra uma issue neste repositório.
