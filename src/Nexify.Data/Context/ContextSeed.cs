using Microsoft.AspNetCore.Identity;
using Nexify.Data.Configuration;
using Nexify.Domain.Entities.Auth;


namespace Nexify.Data.Context
{
    public static class ContextSeed
    {
        public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            await roleManager.CreateAsync(new ApplicationRole(Authorization.Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Authorization.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Authorization.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Authorization.Roles.Basic.ToString()));

            var defaultUser = new ApplicationUser
            {
                UserName = Authorization.default_userName,
                Email = Authorization.default_email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, Authorization.default_password);
                await userManager.AddToRoleAsync(defaultUser, Authorization.default_role.ToString());
            }
        }
    }
}
