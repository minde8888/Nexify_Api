using AutoMapper;
using Microsoft.AspNetCore.Http;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;

namespace Nexify.Service.Services
{
    public class ImagesService : IImagesService
    {
        private readonly IMapper _mapper;
        public ImagesService(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<string> SaveImagesAsync(List<IFormFile> imageFiles)
        {
            if (imageFiles == null)
                throw new FileException("Image file cannot be null.");

            var imagesNames = new List<string>();

            foreach (var file in imageFiles)
            {
                var imageName = await SaveImageAsync(file);
                imagesNames.Add(imageName);
            }

            return string.Join(",", imagesNames);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
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

        public async Task<TDestination> MapAndProcessObjectAsync<TSource, TDestination>(
           TSource sourceObject,
           Func<TSource, IEnumerable<IFormFile>> imagePropertySelector,
           Func<TSource, string> imagePathSelector,
           Func<TSource, string, string> imagePathProcessor,
           string contentRootPath,
           string porpertyName)
        {
            var mappedObject = _mapper.Map<TSource, TDestination>(sourceObject);

            var images = imagePropertySelector(sourceObject);

            if (images != null)
            {
                foreach (var image in images)
                {
                    var imageName = await SaveImagesAsync(new List<IFormFile> { image });
                    SetImageNameProperty(mappedObject, imageName, porpertyName);

                    if (!string.IsNullOrEmpty(contentRootPath))
                    {
                        var fullPath = Path.Combine(contentRootPath, contentRootPath, imageName);
                        await DeleteImageAsync(fullPath);
                    }
                }
            }
            return mappedObject;
        }

        public async Task<TDestination> MapAndProcessObjectListAsync<TSource, TDestination>(
           TSource sourceObject,
           string contentRootPath,
           string propertyName1,
           string propertyName2 = "")
        {
            var mappedObject = _mapper.Map<TSource, TDestination>(sourceObject);

            var itemsImagesProperty = typeof(TSource).GetProperty("ItemsImages");
            var imagesProperty = typeof(TSource).GetProperty("Images");

            var itemsImages = itemsImagesProperty?.GetValue(sourceObject) as List<IFormFile>;
            var images = imagesProperty?.GetValue(sourceObject) as List<IFormFile>;

            if (itemsImages != null)
            {
                await ProcessImagesAsync(mappedObject, itemsImages, contentRootPath, propertyName2);
            }

            if (images != null)
            {
                await ProcessImagesAsync(mappedObject, images, contentRootPath, propertyName1);
            }

            return mappedObject;
        }

        private async Task ProcessImagesAsync<TDestination>(
            TDestination mappedObject,
            IEnumerable<IFormFile> images,
            string contentRootPath,
            string propertyName)
        {
            var imageNamesList = new List<string>();

            foreach (var image in images)
            {
                var imageName = await SaveImageAsync(image);
                imageNamesList.Add(imageName);
                SetImageNamePropertyList(mappedObject, imageName, propertyName);
                if (!string.IsNullOrEmpty(contentRootPath))
                {
                    var fullPath = Path.Combine(contentRootPath, imageName);
                    await DeleteImageAsync(fullPath);
                }
            }
        }

        public async Task<TDestination> MapAndSaveImages<TSource, TDestination>(
              TSource sourceObject,
            List<IFormFile> images,
            string propertyName,
            List<IFormFile> itemsImages = null,
            string propertyItemsNames = "")
        {
            var mappedObject = _mapper.Map<TSource, TDestination>(sourceObject);

            if (images?.Any() == true)
            {
                foreach (var image in images)
                {
                    var imageSaveResult = await SaveImagesAsync(new List<IFormFile> { image });
                    SetImageNamePropertyList(mappedObject, imageSaveResult, propertyName);
                }
            }
            if (itemsImages?.Any() == true)
            {
                foreach (var image in images)
                {
                    var imageSaveResult = await SaveImagesAsync(new List<IFormFile> { image });
                    SetImageNamePropertyList(mappedObject, imageSaveResult, propertyItemsNames);
                }
            }
            return mappedObject;
        }

        public string ProcessImages<T>(T obj, string imageSrc, string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);

            if (propertyInfo != null)
            {
                var propertyValue = (string)propertyInfo.GetValue(obj);
                var modifiedUrl = !string.IsNullOrEmpty(propertyValue)
                    && propertyValue.ToLower() != "null" ?
                    $"{imageSrc}/Images/{propertyValue}" :
                    "";

                propertyInfo.SetValue(obj, modifiedUrl);

                return modifiedUrl;
            }

            return string.Empty;
        }

        private void SetImageNamePropertyList<TDestination>(TDestination obj, string imageName, string propertyName)
        {
            var property = typeof(TDestination).GetProperty(propertyName);
            if (property != null && property.PropertyType == typeof(List<string>))
            {
                var imageNamesList = (List<string>)property.GetValue(obj) ?? new List<string>();

                imageNamesList.Clear();

                imageNamesList.Add(imageName);

                property.SetValue(obj, imageNamesList);
            }
            else
            {
                throw new FileException("ImageNames property is not a List<string>.");
            }
        }

        private void SetImageNameProperty<TDestination>(TDestination obj, string imageName, string porpertyName)
        {
            var property = typeof(TDestination).GetProperty(porpertyName);
            if (property != null && property.PropertyType == typeof(string))
            {
                property.SetValue(obj, imageName);
            }
            else
            {
                throw new FileException($"{porpertyName} property not found.");
            }
        }
    }
}
