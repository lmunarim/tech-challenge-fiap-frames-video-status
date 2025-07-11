using fiap.Application.UseCases;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces;
using Moq;
using Xunit;

namespace fiap.Tests.Application;
public class EmailApplicationTests
{
    [Fact]
    public async Task SendEmailAsync_DeveEnviarEmailComDadosValidos()
    {
        // Arrange
        var secretMock = new SecretEmail
        {
            Smtp = "smtp.exemplo.com",
            User = "teste@exemplo.com",
            Pass = "senha123",
            MailAdress = "remetente@exemplo.com"
        };

        var secretServiceMock = new Mock<ISecretManagerService>();
        secretServiceMock
            .Setup(s => s.ObterSecret<SecretEmail>("dev/fiap/smtp"))
            .ReturnsAsync(secretMock);

        var emailApp = new EmailApplication(secretServiceMock.Object);

        var video = new VideoUpload
        {
            Usuario = new Usuario
            {
                Nome = "Leandro",
                Email = "leandro@exemplo.com"
            },
            StatusUpload = StatusUpload.Finalizado
        };

        // Act
        await emailApp.SendEmailAsync(video);

        // Assert
        secretServiceMock.Verify(s => s.ObterSecret<SecretEmail>("dev/fiap/smtp"), Times.Once);

        // Você pode adicionar asserts com log ou mocks do SmtpClient se usar uma camada de abstração
    }
}
