using Nexify.Data.Context;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class AttributesRepository : IAttributesRepository
    {
        private readonly AppDbContext _context;
        public AttributesRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(Domain.Entities.Attributes.Attributes attributes)
        {
            _context.Attributes.Add(attributes);
            await _context.SaveChangesAsync();
        }

    }
}
