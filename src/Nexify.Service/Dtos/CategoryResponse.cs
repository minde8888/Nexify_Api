
namespace Nexify.Service.Dtos
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ImageSrc { get; set; }
        public IList<SubcategoryResponse> Subcategories { get; set; }
    }
}
