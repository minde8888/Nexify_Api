using System.ComponentModel.DataAnnotations;

namespace Nexify.Domain.Entities.Auth
{
    public class RequestToken
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
