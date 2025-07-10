using fiap.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fiap.Application.Interfaces
{
    public interface IVideoUploadApplication
    {
        Task<VideoUpload> Processar(VideoUpload video);
        Task<List<VideoUpload>> Obter();
        Task<VideoUpload> Obter(string Id);
        Task<VideoUpload> Inserir(VideoUpload video);
        Task<VideoUpload> Atualizar(VideoUpload video);
    }
}
