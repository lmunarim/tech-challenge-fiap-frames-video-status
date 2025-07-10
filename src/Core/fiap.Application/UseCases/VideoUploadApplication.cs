using Amazon.Lambda.Core;
using fiap.Application.Interfaces;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace fiap.Application.UseCases
{
    public class VideoUploadApplication : IVideoUploadApplication
    {
        private readonly ILambdaContext _logger;
        private readonly IVideoUploadRepository _videoUploadRepository;
        public VideoUploadApplication(ILambdaContext logger, IVideoUploadRepository videoUploadRepository)
        {
            _logger = logger;
            _videoUploadRepository = videoUploadRepository;
        }
        public async Task<VideoUpload> Processar(VideoUpload video)
        {
            try
            {
                _logger.Logger.LogInformation($"Verificando existencia de Upload: {video.Id}");
                var up = await Obter(video.Id);

                if (string.IsNullOrEmpty(up.Id))
                {
                    _logger.Logger.LogInformation($"Inserindo novo upload: {video.Id}");
                    return await Inserir(video);
                }

                _logger.Logger.LogInformation($"Atualizando upload existente: { JsonSerializer.Serialize(video)}");
                return await Atualizar(video);
            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao processar video {video.Id}. Erro: {ex.Message}");
                throw;
            }
        }

        public async Task<List<VideoUpload>> Obter()
        {
            try
            {
                _logger.Logger.LogInformation("Buscando lista.");
                return await _videoUploadRepository.Obter();
            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao obter. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<VideoUpload> Obter(string Id)
        {
            try
            {
                _logger.Logger.LogInformation($"Buscando lista de pedidos por Id {Id}");
                return await _videoUploadRepository.Obter(Id);
            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao obter por Id {Id}. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<VideoUpload> Inserir(VideoUpload video)
        {
            try
            {
                _logger.Logger.LogInformation($"Inserindo novo Upload: {video.Id}");
                return await _videoUploadRepository.Inserir(video);
            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao inserir {video.NomeArquivoOrigem}. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<VideoUpload> Atualizar(VideoUpload video)
        {
            try
            {
                _logger.Logger.LogInformation($"Atualizando id: {video.Id}.");
                return await _videoUploadRepository.Atualizar(video);
            }
            catch (Exception ex)
            {
                _logger.Logger.LogError($"Erro ao atualizar id {video.Id}. Erro: {ex.Message}");
                throw;
            }
        }
    }
}
