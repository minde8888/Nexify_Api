using Microsoft.AspNetCore.Identity;
using Nexify.Domain.Entities.Auth;
using Nexify.Service.Interfaces;

namespace Nexify.Service.WrapServices
{
    public class AuthServiceWrap : IAuthServiceWrap
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthServiceWrap(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        public async Task<string> TokenAsync(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task<ApplicationUser> FindUserAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> NewPasswordAsync(ApplicationUser user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }
    }
}
