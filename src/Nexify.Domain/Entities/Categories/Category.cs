using Nexify.Domain.Entities.Products;
using System.ComponentModel.DataAnnotations;


namespace Nexify.Domain.Entities.Categories
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public ICollection<Product> Products { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
