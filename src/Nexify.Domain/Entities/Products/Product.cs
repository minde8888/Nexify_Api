
using Nexify.Domain.Entities.Categories;

namespace Nexify.Domain.Entities.Products
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
