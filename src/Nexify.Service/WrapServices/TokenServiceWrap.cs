using Microsoft.AspNetCore.Identity;
using Nexify.Domain.Entities.Auth;
using Nexify.Service.Interfaces;

namespace Nexify.Service.WrapServices
{
    public class TokenServiceWrap : ITokenServiceWrap
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenServiceWrap(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IList<string>> RolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<ApplicationUser> FindUserIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> FindUserLoginAsync(string loginProvider, string providerKey)
        {
            return await _userManager.FindByLoginAsync(loginProvider, providerKey);
        }
    }
}
