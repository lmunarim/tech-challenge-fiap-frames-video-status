using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using fiap.Domain.Entities;
using fiap.Services;
using Moq;
using System.Text.Json;
using Xunit;

namespace fiap.Tests.Service
{
    public class SecretManagerServiceTests
    {
        [Fact]
        public async Task ObterSecretDbConnect_Test()
        {
            var _logger = new Mock<Serilog.ILogger>();
            var mockSecretsManager = new Mock<IAmazonSecretsManager>();

            SecretEmail email = new SecretEmail { MailAdress = "xx", Pass = "xx", User = "xx", Smtp = "xx" };

            var secretValueResponse = new GetSecretValueResponse
            {
                SecretString = JsonSerializer.Serialize(email)
            };

            mockSecretsManager
                .Setup(sm => sm.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), default))
                .ReturnsAsync(secretValueResponse);

            var secretsManagerClient = mockSecretsManager.Object;

            var response = await secretsManagerClient.GetSecretValueAsync(new GetSecretValueRequest
            {
                SecretId = "segredo"
            });

            var secret = new SecretManagerService(mockSecretsManager.Object);
            var result = await secret.ObterSecret<SecretEmail>("123456");

            // Assert
            Assert.NotNull(result);
        }
    }
}
