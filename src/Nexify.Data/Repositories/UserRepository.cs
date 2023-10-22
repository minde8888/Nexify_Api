using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Auth;
using Nexify.Service.Interfaces;

namespace Nexify.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync<T>(T t)
        {
            await _context.AddAsync(t);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
