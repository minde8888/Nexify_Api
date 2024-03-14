namespace Nexify.Service.Dtos.Category
{
    public class AddCategories
    {
        public string CategoryName { get; set; }
        public List<AddSubcategory> Subcategories { get; set; }
    }
}
