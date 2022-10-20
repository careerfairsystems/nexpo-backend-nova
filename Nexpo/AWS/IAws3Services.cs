using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Nexpo.AWS
{
    public interface IAws3Services
    {
        Task<byte[]> DownloadFileAsync(string file);

        Task<bool> UploadFileAsync(IFormFile file, string name);

        Task<bool> DeleteFileAsync(string fileName);

        bool IfFileExists(string fileName);
    }
}