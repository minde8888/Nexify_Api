using Nexify.Domain.Entities.Auth;

namespace Nexify.Domain.Interfaces
{
    public interface ITokenRepository
    {
        public Task AddTokenAsync(RefreshToken token);
        public Task<RefreshToken> GetToken(string token);
        public Task Update(RefreshToken token);
    }
}
