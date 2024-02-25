
namespace Nexify.Domain.Interfaces
{
    public interface IAttributesReposutory
    {
        public Task AddAsync(Domain.Entities.Attributes.Attribute attributes);
    }
}
