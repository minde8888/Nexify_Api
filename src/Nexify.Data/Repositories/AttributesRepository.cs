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
        public async Task AddAsync(ItemsAttributes attributes)
        {
            _context.ItemsAttributes.Add(attributes);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ItemsAttributes>> GetAllAsync() => await _context.ItemsAttributes.ToListAsync();

        public async Task RemoveAsync(Guid id)
        {
            var attribute = await _context.ItemsAttributes.
                Where(x => x.Id == id).FirstOrDefaultAsync();

            _context.ItemsAttributes.Remove(attribute);
            await _context.SaveChangesAsync();
        }
        public async Task ModifyAsync(ItemsAttributes attribute)
        {
            var currentAttributes = await _context.ItemsAttributes
                .FirstOrDefaultAsync(p => p.Id == attribute.Id);

            _context.Entry(currentAttributes).CurrentValues.SetValues(attribute);
 
            await _context.SaveChangesAsync();
        }
    }
}
