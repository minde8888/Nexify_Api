
using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Entities.SubCategories;

public class Subcategory
{
    public Guid SubCategoryId { get; set; }
    public string SubCategoryName { get; set; }
    public string Description { get; set; }
    public string ImageName { get; set; }
    public List<Product> Products { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime DateUpdated { get; set; }
}
