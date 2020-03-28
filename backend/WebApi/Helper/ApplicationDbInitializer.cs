using Microsoft.AspNetCore.Identity;
using ModelLayer.Models;
using ServiceLayer;

namespace WebApi.Helper
{
    public static class ApplicationDbInitializer
    {
        /// <summary>
        /// Seeding database with admin user and password admin. Also add role 'Admin'
        /// </summary>
        /// <param name="userService">Service to include user</param>
        public static void SeedUsers(IUserService userService)
        {
            if (userService.FindByEmailAsync("admin@admin.com").Result == null)
            {
                User user = new User
                {
                    UserName = "admin",
                    Email = "admin@admin.com"
                };


#pragma warning disable CS0618 // Type or member is obsolete
                IdentityResult result = userService.CreateAsync(user, "@dm1n1stR4tOr").Result;
#pragma warning restore CS0618 // Type or member is obsolete


                if (result.Succeeded)
                {
                    userService.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}