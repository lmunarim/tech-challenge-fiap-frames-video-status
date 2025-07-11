using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using fiap.Repositories;
using Moq;
using Xunit;

namespace fiap.Tests.Repositories
{
    public class VideoUploadRepositoryTests
    {
        [Fact]
        public async Task Obter_DeveRetornarListaComUmUpload()
        {
            var lambdaContextMock = new Mock<ILambdaContext>();
            lambdaContextMock.SetupGet(l => l.Logger).Returns(Mock.Of<ILambdaLogger>());
            var dynamoMock = new Mock<IAmazonDynamoDB>();

            var retorno = new ScanResponse
            {
                Items = new List<Dictionary<string, AttributeValue>>
            {
                new()
                {
                    { "Id", new AttributeValue { S = "abc123" } },
                    { "NomeArquivoOrigem", new AttributeValue { S = "video.mp4" } },
                    { "NomeArquivoGerado", new AttributeValue { S = "video_gerado.mp4" } },
                    { "StatusUpload", new AttributeValue { S = "Finalizado" } },
                    { "UrlS3", new AttributeValue { S = "s3://bucket/video_gerado.mp4" } },
                    { "DataUpload", new AttributeValue { S = "2024-07-10T13:00:00Z" } },
                    {
                        "Usuario", new AttributeValue
                        {
                            M = new Dictionary<string, AttributeValue>
                            {
                                { "Nome", new AttributeValue { S = "Leandro" } },
                                { "Email", new AttributeValue { S = "leandro@email.com" } }
                            }
                        }
                    }
                }
            }
            };

            dynamoMock.Setup(x => x.ScanAsync(
                It.IsAny<ScanRequest>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(retorno);

            var repo = new VideoUploadRepository(dynamoMock.Object);

            // Act
            var resultado = await repo.Obter();

            // Assert
            Assert.Single(resultado);
            Assert.Equal("abc123", resultado[0].Id);
            Assert.Equal("Finalizado", resultado[0].StatusUpload.ToString());
        }
    }

}