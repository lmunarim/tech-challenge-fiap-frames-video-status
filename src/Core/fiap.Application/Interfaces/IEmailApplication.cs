using fiap.Domain.Entities;
using System.Threading.Tasks;

namespace fiap.Application.Interfaces
{
    public interface IEmailApplication
    {
        Task SendEmailAsync(VideoUpload video);
    }
}
