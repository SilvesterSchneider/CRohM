using ModelLayer.Helper;
using ModelLayer.Models;
using ServiceLayer;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace WebApi.Helper
{
    /// <summary>
    /// RAM: 80%
    /// </summary>
    public static class ApplicationDbInitializer
    {
        /// <summary>
        /// Seeding database with admin user and password admin. Also add role 'Admin'
        /// </summary>
        /// <param name="userService">Service to include user</param>
        public static void SeedUsers(IUserService userService)
        {
            User userAdmin = userService.FindByEmailAsync("admin@admin.com").Result;
            if (userAdmin == null)
            {
                User user = new User
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    FirstName = "system",
                    LastName = "admin",
                    IsSuperAdmin = true
                };
                userService.CreateAsync(user, "@dm1n1stR4tOr").Wait();
            }
            userAdmin = userService.FindByEmailAsync("admin@admin.com").Result;
            if (userAdmin != null)
            {
                if (!userService.GetRolesAsync(userAdmin).Result.Contains(RoleClaims.ADMIN_GROUP))
                {
                    userService.AddToRoleAsync(userAdmin, RoleClaims.ADMIN_GROUP).Wait();
                }
                IList<Claim> existingClaims = userService.GetClaimsAsync(userAdmin).Result;
                foreach (Claim claim in RoleClaims.GetAllAdminClaims())
                {
                    if (existingClaims.FirstOrDefault(x => claim.Type.Equals(x.Type) && claim.Value.Equals(x.Value)) == null)
                    {
                        userService.AddClaimAsync(userAdmin, claim).Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Erzeugen aller standart rollen (Admin und Datenschutzbeauftragter mit den jeweiligen rechten)
        /// </summary>
        /// <param name="roleService">rollen service</param>
        public static void SeedRoles(IRoleService roleService)
        {
            foreach (string roleToCreate in RoleClaims.DEFAULT_GROUPS)
            {
                Role role = roleService.FindRoleByNameAsync(roleToCreate).Result;
                if (role == null)
                {
                    roleService.CreateAsync(new Role() { Name = roleToCreate }).Wait();
                }
                role = roleService.FindRoleByNameAsync(roleToCreate).Result;
                if (role != null)
                {
                    IList<Claim> existingClaims = roleService.GetClaimsAsync(role).Result;
                    List<Claim> claimsToCheck = new List<Claim>();
                    if (role.Name.Equals(RoleClaims.ADMIN_GROUP))
                    {
                        claimsToCheck = RoleClaims.GetAllAdminClaims();
                    }
                    else if (role.Name.Equals(RoleClaims.DATA_SECURITY_ENGINEER_GROUP))
                    {
                        claimsToCheck = RoleClaims.GetAllDsgvoClaims();
                    }
                    foreach (Claim claim in claimsToCheck)
                    {
                        if (existingClaims.FirstOrDefault(x => x.Type.Equals(claim.Type) && x.Value.Equals(claim.Value)) == null)
                        {
                            roleService.AddClaimAsync(role, claim).Wait();
                        }
                    }
                }
            }
        }
    }
}
