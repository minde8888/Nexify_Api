namespace Nexify.Service.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Context { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Stok { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<string> ImageSrc { get; set; }

    }
}
