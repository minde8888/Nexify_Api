namespace Nexify.Service.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public ICollection<string> ImageSrc { get; set; }

    }
}
