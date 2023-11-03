using System.ComponentModel.DataAnnotations;

namespace Nexify.Domain.Entities.Auth
{
    public class Login
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
