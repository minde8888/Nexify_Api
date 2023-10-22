using Nexify.Domain.Entities.Auth;

namespace Nexify.Service.Interfaces
{
    public interface IUserRepository
    {
        public Task AddUserAsync<T>(T t);
        public Task<ApplicationUser> GetUserByEmail(string email);
    }
}
