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

            var originalNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName);
            var uniqueImageName = $"{Guid.NewGuid()}_{originalNameWithoutExtension}{fileExtension}";
            var imagePath = Path.Combine("Images", uniqueImageName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return uniqueImageName;
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
           string propertyName,
           List<string> imagesNames,
           List<int> indices = null)
        {
            var mappedObject = _mapper.Map<TSource, TDestination>(sourceObject);

            var imagesProperty = typeof(TSource).GetProperty("Images");

            var images = imagesProperty?.GetValue(sourceObject) as List<IFormFile>;

            if (images != null)
            {
                var imageNamesList = await ProcessImagesAsync(mappedObject, images, contentRootPath, propertyName);
                var mergedList = new List<string>();
                if (imagesNames != null && imagesNames.Any())
                {
                    mergedList = MergeWithDesignatedPositions(imagesNames, imageNamesList, indices);
                }
                var propertyToSet = typeof(TDestination).GetProperty(propertyName);
                if (propertyToSet != null && propertyToSet.CanWrite)
                {
                    propertyToSet.SetValue(mappedObject, mergedList);
                }
            }

            return mappedObject;
        }
        private static List<string> MergeWithDesignatedPositions(List<string> firstList, List<string> secondList, List<int> indicesList)
        {
            if (secondList.Count != indicesList.Count)
            {
                throw new FileException("The length of secondList should match the length of indicesList.");
            }

            var indexedElements = new Dictionary<int, string>();

            for (int i = 0; i < secondList.Count; i++)
            {
                indexedElements[indicesList[i]] = secondList[i];
            }

            int currentIndex = 0;
            foreach (var item in firstList)
            {
                while (indexedElements.ContainsKey(currentIndex))
                {
                    currentIndex++;
                }
                indexedElements[currentIndex] = item;
                currentIndex++;
            }

            return indexedElements.OrderBy(element => element.Key).Select(element => element.Value).ToList();
        }

        private async Task<List<string>> ProcessImagesAsync<TDestination>(
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

            return imageNamesList;
        }

        public async Task<TDestination> MapAndSaveImages<TSource, TDestination>(
            TSource sourceObject,
            List<IFormFile> images,
            string propertyName)
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
