using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using fiap.Application.Interfaces;
using fiap.Application.UseCases;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces;
using fiap.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaStatus
{
    public class Function
    {
        private readonly ServiceProvider _serviceProvider;
        public Function()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        public async Task<Task> FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            try
            {
                context.Logger.LogInformation("Iniciando processamento da requisição");

                context.Logger.LogInformation($"Recebendo requisição {JsonSerializer.Serialize(evnt)}");

                using var scope = _serviceProvider.CreateScope();
                var videoUploadApplication = scope.ServiceProvider.GetRequiredService<IVideoUploadApplication>();

                foreach (var record in evnt.Records)
                {
                    context.Logger.LogInformation($"Processing message {record.MessageId} with body: {record.Body}");
                    await videoUploadApplication.Processar(JsonSerializer.Deserialize<VideoUpload>(record.Body));
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Erro ao processar requisição: {ex.Message}");
                throw;
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {

            serviceCollection.AddLogging();
            serviceCollection.AddLogging(builder => builder.AddLambdaLogger());
            serviceCollection.AddAWSService<IAmazonDynamoDB>();
            serviceCollection.AddTransient<IVideoUploadApplication, VideoUploadApplication>();
            serviceCollection.AddTransient<IVideoUploadRepository, VideoUploadRepository>();
        }
   }

    
}
