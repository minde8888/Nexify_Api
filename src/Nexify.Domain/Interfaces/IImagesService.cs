using Microsoft.AspNetCore.Http;

namespace Nexify.Domain.Interfaces
{
    public interface IImagesService
    {
        Task<string> SaveImages(List<IFormFile> imageFiles);
        Task DeleteImageAsync(string imagePath);
        Task<TDestination> MapAndProcessObjectAsync<TSource, TDestination>(
            TSource sourceObject,
            Func<TSource, IEnumerable<IFormFile>> imagePropertySelector,
            Func<TSource, string> imagePathSelector,
            Func<TSource, string, string> imagePathProcessor,
            string contentRootPath);
        Task<TDestination> MapAndProcessObjectListAsync<TSource, TDestination>(
            TSource sourceObject,
            Func<TSource, IEnumerable<IFormFile>> imagePropertySelector,
            Func<TSource, string> imagePathSelector,
            Func<TSource, string, string> imagePathProcessor,
            string contentRootPath);        
        Task<TDestination> MapAndSaveImages<TSource, TDestination>(TSource sourceObject, List<IFormFile> images);
        string ProcessImages<T>(T obj, string imageSrc);
    }
}
