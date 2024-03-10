using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Entities.Attributes
{
    public class ItemsAttributes
    {
        public Guid Id { get; set; }
        public string AttributeName { get; set; }
        public string ImageName { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
