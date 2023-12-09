using Nexify.Domain.Entities.Products;
using Nexify.Domain.Entities.Subcategories;
using System.ComponentModel.DataAnnotations;

namespace Nexify.Domain.Entities.Categories;

public class Category
{
    [Key]
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public string ImageName { get; set; }
    public IList<Subcategory> Subcategories { get; set; }
    public ICollection<Product> Products { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime DateUpdated { get; set; }
    public bool IsDeleted { get; set; } = false;
}
