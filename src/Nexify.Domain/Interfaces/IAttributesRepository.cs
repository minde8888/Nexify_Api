using Nexify.Domain.Entities.Attributes;

namespace Nexify.Domain.Interfaces
{
    public interface IAttributesRepository
    {
        public Task AddAsync(Attributes attributes);
    }
}
