using Nexify.Domain.Entities.Auth;

namespace Nexify.Service.Interfaces
{
    public interface ITokenServiceWrap
    {
        Task<IList<string>> RolesAsync(ApplicationUser user);
        Task<ApplicationUser> FindUserIdAsync(string id);
        Task<ApplicationUser> FindUserLoginAsync(string loginProvider, string providerKey);
    }
}
