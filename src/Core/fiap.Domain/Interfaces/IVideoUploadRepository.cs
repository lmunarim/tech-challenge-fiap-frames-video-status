using fiap.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fiap.Domain.Interfaces
{
    public interface IVideoUploadRepository
    {
        Task<List<VideoUpload>> Obter();
        Task<VideoUpload> Obter(string Id);
        Task<VideoUpload> Inserir(VideoUpload videoUpload);
        Task<VideoUpload> Atualizar(VideoUpload videoUpload);
    }
}
