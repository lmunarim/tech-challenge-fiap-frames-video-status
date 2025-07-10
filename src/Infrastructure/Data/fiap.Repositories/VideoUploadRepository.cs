using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces;

namespace fiap.Repositories
{
    public class VideoUploadRepository : IVideoUploadRepository
    {
        private readonly ILambdaContext _logger;
        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private const string FIAP_VIDEO_UPLOAD_DYNAMODB = "fiap-video-upload";

        public VideoUploadRepository(ILambdaContext logger, IAmazonDynamoDB amazonDynamoDb)
        {
            _logger = logger;
            _amazonDynamoDb = amazonDynamoDb;
        }
        public async Task<List<VideoUpload>> Obter()
        {
            var lst = new List<VideoUpload>();
            try
            {
                var scanRequest = new ScanRequest
                {
                    TableName = FIAP_VIDEO_UPLOAD_DYNAMODB
                };
                var queryResponse = await _amazonDynamoDb.ScanAsync(scanRequest);

                foreach (var item in queryResponse.Items)
                {
                    lst.Add(new VideoUpload
                    {
                        Id = item["Id"].S,
                        NomeArquivoOrigem = item["NomeArquivoOrigem"].S,
                        NomeArquivoGerado = item["NomeArquivoGerado"].S,
                        StatusUpload = Enum.Parse<StatusUpload>(item["StatusUpload"].S),
                        UrlS3 = item["UrlS3"].S,
                        Usuario = new Usuario
                        {
                            Nome = item["Usuario"].M["Nome"].S,
                            Email = item["Usuario"].M["Email"].S
                        },
                        DataUpload = DateTime.Parse(item["DataUpload"].S)
                    });
                }

                _logger.Logger.LogInformation("Lista de uploads obtida com sucesso!");

                return await Task.FromResult(lst);
            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao obter uploads. Erro: {ex.Message}.");
                throw;
            }
        }
        public async Task<VideoUpload> Obter(string Id)
        {
            try
            {
                var queryRequest = new GetItemRequest
                {
                    TableName = FIAP_VIDEO_UPLOAD_DYNAMODB,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue { S = Id } }
                    }
                };

                var response = await _amazonDynamoDb.GetItemAsync(queryRequest);

                if (response.Item == null || response.Item.Count == 0)
                    throw new Exception($"Id {Id} não encontrado.");

                var item = response.Item;

                var videoUpload = new VideoUpload
                {
                    Id = item["Id"].S,
                    NomeArquivoOrigem = item["NomeArquivoOrigem"].S,
                    NomeArquivoGerado = item["NomeArquivoGerado"].S,
                    StatusUpload = Enum.Parse<StatusUpload>(item["StatusUpload"].S),
                    UrlS3 = item["UrlS3"].S,
                    Usuario = new Usuario
                    {
                        Nome = item["Usuario"].M["Nome"].S,
                        Email = item["Usuario"].M["Email"].S
                    },
                    DataUpload = DateTime.Parse(item["DataUpload"].S)
                };

                _logger.Logger.LogInformation("Lista de uploads obtida com sucesso!");

                return videoUpload;
            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao obter uploads. Erro: {ex.Message}.");
                throw;
            }
        }
        public Task<VideoUpload> Inserir(VideoUpload videoUpload)
        {
            try
            {
                var queryRequest = new PutItemRequest
                {
                    TableName = FIAP_VIDEO_UPLOAD_DYNAMODB,
                    Item = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue { S = videoUpload.Id } },
                        { "NomeArquivoOrigem", new AttributeValue { S = videoUpload.NomeArquivoOrigem } },
                        { "NomeArquivoGerado", new AttributeValue { S = videoUpload.NomeArquivoGerado } },
                        { "StatusUpload", new AttributeValue { S = videoUpload.StatusUpload.ToString() } },
                        { "UrlS3", new AttributeValue { S = videoUpload.UrlS3 } },
                        { "Usuario", new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    { "Nome", new AttributeValue { S = videoUpload.Usuario.Nome } },
                                    { "Email", new AttributeValue { S = videoUpload.Usuario.Email } }
                                }
                            }
                        },
                        { "DataUpload", new AttributeValue { S = videoUpload.DataUpload.ToString("o") } }

                    }
                };

                _amazonDynamoDb.PutItemAsync(queryRequest).Wait();

                _logger.Logger.LogInformation($"Upload {videoUpload.Id} inserido com sucesso!");
                return Task.FromResult(videoUpload);

            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao salvar {videoUpload.Id}. Erro: {ex.Message}.");
                throw;
            }
        }
        public Task<VideoUpload> Atualizar(VideoUpload videoUpload)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = FIAP_VIDEO_UPLOAD_DYNAMODB,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "Id", new AttributeValue { S = videoUpload.Id } }
                    },
                    AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                    {
                        { "StatusUpload", new AttributeValueUpdate {Action = AttributeAction.PUT, Value = new AttributeValue { S = videoUpload.StatusUpload.ToString() } } },
                        { "UrlS3", new AttributeValueUpdate {Action = AttributeAction.PUT, Value = new AttributeValue { S = videoUpload.UrlS3 } } },
                        { "NomeArquivoGerado", new AttributeValueUpdate {Action = AttributeAction.PUT, Value = new AttributeValue { S = videoUpload.NomeArquivoGerado } } },
                    }
                };

                _amazonDynamoDb.UpdateItemAsync(request).Wait();
                
                _logger.Logger.LogInformation($"Sucesso ao atualizar id: {videoUpload.Id}");

                return Task.FromResult(videoUpload);
            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao atualizar id: {videoUpload.Id}. Erro: {ex.Message}");
                throw;
            }
        }
    }
}


