namespace Nexify.Service.Dtos.Category
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageSrc { get; set; }
        public DateTime DateCreated { get; set; }
        public IList<SubcategoryResponse> Subcategories { get; set; }
    }
}

