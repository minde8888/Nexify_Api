
using Microsoft.AspNetCore.Http;

namespace Nexify.Domain.Interfaces
{
    public interface IImagesService
    {
        public Task<string> SaveImages(List<IFormFile> imageFiles);
        public Task DeleteImageAsync(string imagePath);
    }
}
