using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Attributes;
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
        public async Task AddAsync(Attributes attributes)
        {
            _context.Attributes.Add(attributes);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Attributes>> GetAllAsync() => await _context.Attributes.ToListAsync();

        public async Task RemoveAsync(Guid id)
        {
            var attribute = await _context.Attributes.
                Where(x => x.Id == id).FirstOrDefaultAsync();

            _context.Attributes.Remove(attribute);
            await _context.SaveChangesAsync();
        }

    }
}
