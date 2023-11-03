using System.ComponentModel.DataAnnotations;

namespace Nexify.Domain.Entities.Auth
{
    public class ForgotPassword
    {
        [Required]
        public string Email { get; set; }
    }
}
