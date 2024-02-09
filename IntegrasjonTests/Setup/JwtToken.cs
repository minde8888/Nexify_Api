using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nexify.Domain.Entities.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IntegrasjonTests.Setup
{
    public class JwtToken
    {
        private readonly JwtConfig _jwtConfig;

        public JwtToken(IConfiguration configuration)
        {
            _jwtConfig = new JwtConfig
            {
                Secret = configuration["JwtConfig:Secret"]
            };
        }

        public string GenerateJwtToken(string userEmail, string userRole)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new Claim("Id", Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userEmail),
            new Claim(JwtRegisteredClaimNames.Sub, userEmail),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, userRole)
        });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public JwtConfig Secret()
        {
            return _jwtConfig;
        }
    }
}
