using Microsoft.AspNetCore.Http;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;

namespace Nexify.Service.Services
{
    public class ImagesService : IImagesService
    {
        public async Task<string> SaveImages(List<IFormFile> imageFiles)
        {
            if (imageFiles == null)
                throw new FileException("Image file cannot be null.");

            var imageNames = new List<string>();

            foreach (var file in imageFiles)
            {
                var imageName = await SaveImage(file);
                imageNames.Add(imageName);
            }

            return string.Join(",", imageNames);
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new FileException("Image file cannot be null.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var fileExtension = Path.GetExtension(imageFile.FileName);

            if (!allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                throw new FileException("File formant is not allowed.");
            }

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var imagePath = Path.Combine("Images", imageName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return imageName;
        }

        public async Task DeleteImageAsync(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                await Task.Run(() => File.Delete(imagePath));
            }
        }
    }
}
