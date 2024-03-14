namespace Nexify.Service.Dtos.Category
{
    public class CategoryProducts
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public List<string> ImageSrc { get; set; }
    }
}
