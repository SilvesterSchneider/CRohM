using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using ModelLayer.Models;
using RepositoryLayer;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
            User userAdmin = userService.FindByEmailAsync("admin@admin.com").Result;
            if (userAdmin == null)
            {
                User user = new User
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    FirstName = "system",
                    LastName = "admin"
                };
                IdentityResult result = userService.CreateAsync(user, "@dm1n1stR4tOr").Result;
                if (result.Succeeded)
                {
                    userService.AddToRoleAsync(user, "Admin").Wait();
                }
            }
            userAdmin = userService.FindByEmailAsync("admin@admin.com").Result;
            if (userAdmin != null)
            {
                List<Claim> allClaims = RoleClaims.GetAllClaims();
                if (userService.GetClaimsAsync(userAdmin).Result.Count != allClaims.Count)
                {
                    foreach (Claim claim in allClaims)
                    {
                        userService.AddClaimAsync(userAdmin, claim).Wait();
                    }
                }
            }
        }

        public static void SeedRoles(IRoleService roleService)
        {
            Role role = roleService.FindRoleByNameAsync("Admin").Result;
            if (role == null)
            {
                roleService.CreateAsync(new Role() { Name = "Admin" }).Wait();
            }
            role = roleService.FindRoleByNameAsync("Admin").Result;
            List<Claim> allClaims = RoleClaims.GetAllClaims();
            if (role != null && roleService.GetClaimsAsync(role).Result.Count != allClaims.Count)
            {
                foreach (Claim claim in RoleClaims.GetAllClaims())
                {
                    roleService.AddClaimAsync(role, claim).Wait();
                }
            }
        }
    }
}
