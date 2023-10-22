using Microsoft.IdentityModel.Tokens;
using Nexify.Domain.Entities.Auth;
using System.Security.Claims;

namespace Nexify.Domain.Interfaces
{
    public interface ITokenService
    {
        public RefreshToken GetRefreshToken(SecurityToken token, string rand, ApplicationUser user);
        public Task<AuthResults> GenerateJwtTokenAsync(ApplicationUser user);
        public Task<AuthResults> VerifyToken(RequestToken tokenRequest, ClaimsPrincipal principal, SecurityToken validatedToken);
    }
}
