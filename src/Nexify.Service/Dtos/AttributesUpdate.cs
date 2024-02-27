using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class AttributesUpdate
    {
        public Guid Id { get; set; }
        public string AttributeName { get; set; }
        public string ImageName { get; set; }   
        public IFormFile Image { get; set; }
    }
}
