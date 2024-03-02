using Microsoft.AspNetCore.Http;
using Nexify.Domain.Entities.Attributes;

namespace Nexify.Domain.Interfaces
{
    public interface IImagesService
    {
        Task<string> SaveImagesAsync(List<IFormFile> imageFiles);
        Task DeleteImageAsync(string imagePath);
        Task<TDestination> MapAndProcessObjectAsync<TSource, TDestination>(
            TSource sourceObject,
            Func<TSource, IEnumerable<IFormFile>> imagePropertySelector,
            Func<TSource, string> imagePathSelector,
            Func<TSource, string, string> imagePathProcessor,
            string contentRootPath,
            string propertyName);
        Task<TDestination> MapAndProcessObjectListAsync<TSource, TDestination>(
           TSource sourceObject,
           string contentRootPath,
           string propertyName1,
           string propertyName2 = "");
        Task<TDestination> MapAndSaveImages<TSource, TDestination>(
            TSource sourceObject,
            List<IFormFile> images,
            string propertyName,
            List<IFormFile> itemsImages = null,
            string propertyItemsNames = "");
        string ProcessImages<T>(T obj, string imageSrc, string propertyName);
    }
}
