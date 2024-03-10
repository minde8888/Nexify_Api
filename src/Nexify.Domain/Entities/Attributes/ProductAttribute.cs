
using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Entities.Attributes
{
    public class ProductAttribute
    {
        public Guid AtributesId { get; set; }
        public ItemsAttributes ItemsAttributes { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
