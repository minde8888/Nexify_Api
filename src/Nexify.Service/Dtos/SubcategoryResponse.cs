namespace Nexify.Service.Dtos
{
    public class SubcategoryResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageSrc { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
