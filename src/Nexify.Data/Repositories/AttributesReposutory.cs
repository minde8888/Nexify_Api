using Nexify.Data.Context;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class AttributesReposutory : IAttributesReposutory
    {
        private readonly AppDbContext _context;
        public AttributesReposutory(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(Domain.Entities.Attributes.Attribute attributes)
        {
            _context.Attributes.Add(attributes);
            await _context.SaveChangesAsync();
        }

    }
}
