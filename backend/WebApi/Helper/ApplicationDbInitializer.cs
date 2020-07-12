using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using ModelLayer.Models;
using RepositoryLayer;
using ServiceLayer;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Helper
{
    public static class ApplicationDbInitializer
    {
        /// <summary>
        /// Seeding database with admin user and password admin. Also add role 'Admin'
        /// </summary>
        /// <param name="userService">Service to include user</param>
        public static void SeedUsers(IUserService userService, IPermissionGroupService permissionService)
        {
            if (userService.FindByEmailAsync("admin@admin.com").Result == null)
            {
                User user = new User
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    Permission = new List<PermissionGroup>()
                    LastName = "admin",
                    Email = "admin@admin.com"
                };

                List <PermissionGroup> allpermissionGroups =  permissionService.GetAllPermissionGroupAsync().Result;
  
                user.Permission.Add(allpermissionGroups.FirstOrDefault(x => x.Id == 1));

                IdentityResult result = userService.CreateAsync(user, "@dm1n1stR4tOr").Result;
            }
        }

        public static void SeedPermissions(IPermissionGroupService permissionGroupService, IMapper mapper) {

            List<PermissionGroup> groups = permissionGroupService.GetAllPermissionGroupAsync().Result;
            bool adminexists = false;

            foreach (PermissionGroup group in groups) {
                if (group.Id == 1) {
                    adminexists = true;
                    break;
                }
            }

            if (!adminexists) {
                permissionGroupService.CreatePermissionGroupAsync(GetAdminPermissions());
            }
        }

        public static PermissionGroup GetAdminPermissions() {
            PermissionGroup admin = new PermissionGroup();
            admin.Name = "Admin";
            admin.Id = 0;
            admin.Permissions.AddRange(AllRoles.GetAllRoles());

            return admin;
        }
    }
}