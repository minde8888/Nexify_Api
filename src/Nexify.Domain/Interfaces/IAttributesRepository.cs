using Nexify.Domain.Entities.Attributes;

namespace Nexify.Domain.Interfaces
{
    public interface IAttributesRepository
    {
        public Task AddAsync(Attributes attributes);
        public Task<List<Attributes>> GetAllAsync();
        public Task RemoveAsync(Guid id);
        public Task ModifyAsync(Attributes attribute);
    }
}
