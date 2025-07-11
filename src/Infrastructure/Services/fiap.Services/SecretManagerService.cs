using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using fiap.Domain.Interfaces;
using System.Text.Json;

namespace fiap.Services
{
    public class SecretManagerService : ISecretManagerService
    {
        private readonly IAmazonSecretsManager _secret;
        public SecretManagerService(IAmazonSecretsManager secret)
        {
            _secret = secret;
        }
        public async Task<T> ObterSecret<T>(string segredo)
        {
            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = segredo,
                VersionStage = "AWSCURRENT"
            };

            GetSecretValueResponse response;

            try
            {
                response = await _secret.GetSecretValueAsync(request);
                Console.WriteLine("Secret obtida com sucesso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter a secret - {ex.Message}");
                throw;
            }

            return JsonSerializer.Deserialize<T>(response.SecretString);
        }
    }
}
