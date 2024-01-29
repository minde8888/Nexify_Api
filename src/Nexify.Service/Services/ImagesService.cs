﻿using AutoMapper;
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
                    var imageName = await SaveImages(new List<IFormFile> { image });
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
           Func<TSource, IEnumerable<IFormFile>> imagePropertySelector,
           string contentRootPath,
           string propertyName)
        {
            var mappedObject = _mapper.Map<TSource, TDestination>(sourceObject);

            var images = imagePropertySelector(sourceObject);

            if (images != null)
            {
                foreach (var image in images)
                {
                    var imageName = await SaveImages(new List<IFormFile> { image });
                    SetImageNamePropertyList(mappedObject, imageName, propertyName);

                    if (!string.IsNullOrEmpty(contentRootPath))
                    {
                        var fullPath = Path.Combine(contentRootPath, contentRootPath, imageName);
                        await DeleteImageAsync(fullPath);
                    }
                }
            }
            return mappedObject;
        }

        public async Task<TDestination> MapAndSaveImages<TSource, TDestination>(TSource sourceObject, List<IFormFile> images, string propertyName)
        {
            var mappedObject = _mapper.Map<TSource, TDestination>(sourceObject);

            if (images != null)
            {
                foreach (var image in images)
                {
                    var imageSaveResult = await SaveImages(new List<IFormFile> { image });
                    SetImageNamePropertyList(mappedObject, imageSaveResult, propertyName);
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
                throw new FileException("ImageName property not found.");
            }
        }

    }
}
