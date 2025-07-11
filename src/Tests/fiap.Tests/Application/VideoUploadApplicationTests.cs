using Amazon.Lambda.Core;
using fiap.Application.UseCases;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces;
using Moq;
using Xunit;

namespace fiap.Tests.Application
{
    public class VideoUploadApplicationTests
    {
        [Fact]
        public async Task Processar_DeveInserirUpload_QuandoNaoEncontrado()
        {
            // Arrange
            var lambdaContextMock = new Mock<ILambdaContext>();
            lambdaContextMock.SetupGet(l => l.Logger).Returns(Mock.Of<ILambdaLogger>());

            var repoMock = new Mock<IVideoUploadRepository>();

            var video = new VideoUpload { Id = "123", NomeArquivoOrigem = "origem.mp4" };

            repoMock.Setup(r => r.Obter("123")).ReturnsAsync(new VideoUpload()); // Simula não encontrado
            repoMock.Setup(r => r.Inserir(video)).ReturnsAsync(video);

            var app = new VideoUploadApplication(repoMock.Object);

            // Act
            var result = await app.Processar(video);

            // Assert
            Assert.Equal("123", result.Id);
            repoMock.Verify(r => r.Inserir(video), Times.Once);
            repoMock.Verify(r => r.Atualizar(video), Times.Never);
        }

        [Fact]
        public async Task Processar_DeveAtualizarUpload_QuandoJaExiste()
        {
            // Arrange
            var lambdaContextMock = new Mock<ILambdaContext>();
            lambdaContextMock.SetupGet(l => l.Logger).Returns(Mock.Of<ILambdaLogger>());

            var repoMock = new Mock<IVideoUploadRepository>();

            var video = new VideoUpload { Id = "456", NomeArquivoOrigem = "origem.mp4" };

            repoMock.Setup(r => r.Obter("456")).ReturnsAsync(video); // Simula encontrado
            repoMock.Setup(r => r.Atualizar(video)).ReturnsAsync(video);

            lambdaContextMock.SetupSequence(x=>x.Logger.LogInformation(It.IsAny<string>()));

            var app = new VideoUploadApplication(repoMock.Object);

            // Act
            var result = await app.Processar(video);

            // Assert
            Assert.Equal("456", result.Id);
            repoMock.Verify(r => r.Atualizar(video), Times.Once);
            repoMock.Verify(r => r.Inserir(video), Times.Never);
        }
    }

}
