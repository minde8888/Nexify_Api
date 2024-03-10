using Nexify.Domain.Entities.Attributes;

namespace Nexify.Domain.Interfaces
{
    public interface IAttributesRepository
    {
        public Task AddAsync(ItemsAttributes attributes);
        public Task<List<ItemsAttributes>> GetAllAsync();
        public Task RemoveAsync(Guid id);
        public Task ModifyAsync(ItemsAttributes attribute);
    }
}
