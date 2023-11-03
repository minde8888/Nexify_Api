using Microsoft.AspNetCore.Identity;
using Nexify.Domain.Entities.Auth;

namespace Nexify.Service.Interfaces
{
    public interface IAuthRepository
    {
        bool UserExistAsync(string phoneNumber, string email);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> CreateBasicUser(ApplicationUser user);
        Task AddRoleAsync(ApplicationUser user, string role);
        Task<ApplicationUser> AuthUserAsync(string email);
        Task<bool> PasswordValidatorAsync(ApplicationUser user, string password);
        Task<IList<string>> RolesAsync(ApplicationUser user);
    }
}
