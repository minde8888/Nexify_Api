using Microsoft.AspNetCore.Identity;
using Nexify.Domain.Entities.Auth;

namespace Nexify.Service.Interfaces
{
    public interface IAuthServiceWrap
    {
        public Task<string> TokenAsync(ApplicationUser user);
        public Task UpdateUserAsync(ApplicationUser user);
        public Task<ApplicationUser> FindUserAsync(string email);
        public Task<IdentityResult> NewPasswordAsync(ApplicationUser user, string token, string password);
    }
}
