using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class AttributesRequest
    {
        public string AttributeName { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> ImageDescription { get; set; }
        public List<string> ImagesNames { get; set; }
    }
}
