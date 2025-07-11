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
        private readonly IVideoUploadRepository _videoUploadRepository;
        public VideoUploadApplication(IVideoUploadRepository videoUploadRepository)
        {
            _videoUploadRepository = videoUploadRepository;
        }
        public async Task<VideoUpload> Processar(VideoUpload video)
        {
            try
            {
                Console.WriteLine($"Verificando existencia de Upload: {video.Id}");
                var up = await Obter(video.Id);

                if (string.IsNullOrEmpty(up.Id))
                {
                    Console.WriteLine($"Inserindo novo upload: {video.Id}");
                    return await Inserir(video);
                }

                Console.WriteLine($"Atualizando upload existente: { JsonSerializer.Serialize(video)}");
                return await Atualizar(video);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar video {video.Id}. Erro: {ex.Message}");
                throw;
            }
        }

        public async Task<List<VideoUpload>> Obter()
        {
            try
            {
                Console.WriteLine("Buscando lista.");
                return await _videoUploadRepository.Obter();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<VideoUpload> Obter(string Id)
        {
            try
            {
                Console.WriteLine($"Buscando lista de pedidos por Id {Id}");
                return await _videoUploadRepository.Obter(Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter por Id {Id}. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<VideoUpload> Inserir(VideoUpload video)
        {
            try
            {
                Console.WriteLine($"Inserindo novo Upload: {video.Id}");
                return await _videoUploadRepository.Inserir(video);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir {video.NomeArquivoOrigem}. Erro: {ex.Message}");
                throw;
            }
        }
        public async Task<VideoUpload> Atualizar(VideoUpload video)
        {
            try
            {
                Console.WriteLine($"Atualizando id: {video.Id}.");
                return await _videoUploadRepository.Atualizar(video);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar id {video.Id}. Erro: {ex.Message}");
                throw;
            }
        }
    }
}
