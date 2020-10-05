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
                    userService.AddToRoleAsync(user, RoleClaims.ADMIN_GROUP).Wait();
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

        /// <summary>
        /// Erzeugen aller standart rollen (Admin und Datenschutzbeauftragter)
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
                List<Claim> allClaims = RoleClaims.GetAllClaims();
                if (role != null && role.Name.Equals(RoleClaims.ADMIN_GROUP) && roleService.GetClaimsAsync(role).Result.Count != allClaims.Count)
                {
                    foreach (Claim claim in allClaims)
                    {
                        roleService.AddClaimAsync(role, claim).Wait();
                    }
                }
                else if (role != null && role.Name.Equals(RoleClaims.DATA_SECURITY_ENGINEER_GROUP) && roleService.GetClaimsAsync(role).Result.Count == 0)
                {
                    foreach (Claim claim in RoleClaims.GetAllDsgvoClaims())
                    {
                        roleService.AddClaimAsync(role, claim).Wait();
                    }
                }
            }
<<<<<<< HEAD
        }

        public static PermissionGroup GetAdminPermissions() {
            PermissionGroup admin = new PermissionGroup();
            admin.Name = "Admin";
            admin.Id = 0;
            admin.Permissions.AddRange(AllRoles.AdminPermissions());

            return admin;
=======
            
>>>>>>> 2f7fe36f5b41f64229b5196b868fde44da257158
        }

        public static PermissionGroup GetDatenschutzBeaftragterPermissions()
        {
            PermissionGroup datenschutzbeauftragter = new PermissionGroup();
            datenschutzbeauftragter.Name = "Datenschutzbeauftragter";
            datenschutzbeauftragter.Id = 1;
            datenschutzbeauftragter.Permissions.AddRange(AllRoles.DatenschutzBeauftragterPermissions());

            return datenschutzbeauftragter;
        }
    }
}
