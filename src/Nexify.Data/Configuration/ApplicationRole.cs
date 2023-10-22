using Microsoft.AspNetCore.Identity;

namespace Nexify.Data.Configuration
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole() : base()
        {
        }
        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
