﻿namespace Nexify.Service.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Size { get; set; }
        public string Stok { get; set; }
        public string Location { get; set; }
        public List<string> ImageNames { get; set; }
        public List<string> ImageSrc { get; set; }
        public List<string> ItemsImagesNames { get; set; }
        public List<string> ItemSrc { get; set; }
        public DateTime DateCreated { get; set; }
        public List<CategoryResponse> Catrgories { get; set; }       
    }
}
