
namespace Nexify.Service.Dtos
{
    public class AddCategories
    {
        public string CategoryName { get; set; }
        public List<AddSubcategory> Subcategories { get; set; }
    }
}
