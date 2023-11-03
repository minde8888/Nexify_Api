using System.ComponentModel.DataAnnotations;

namespace Nexify.Domain.Entities.Auth
{
    public class ResetPassword
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
