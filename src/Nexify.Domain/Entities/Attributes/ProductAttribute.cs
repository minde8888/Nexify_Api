﻿
namespace Nexify.Domain.Entities.Attributes
{
    public class ProductAttribute
    {
        public Guid Id { get; set; }
        public List<string> AttributeName { get; set; }
        public List<string> ImageDescription { get; set; }
        public List<string> ImagesNames { get; set; }
    }
}
