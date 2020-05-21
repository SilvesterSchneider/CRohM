using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using ModelLayer.Models;
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
                };

                List <PermissionGroup> allpermissionGroups =  permissionService.GetAllPermissionGroupAsync().Result;

                user.Permission.Add(allpermissionGroups.FirstOrDefault(x => x.Id == 1));

                IdentityResult result = userService.CreateAsync(user, "@dm1n1stR4tOr").Result;

                if (result.Succeeded)
                {
                    userService.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        public static void SeedPermissions(IPermissionGroupService permissionGroupService, IMapper mapper) {

            List<PermissionGroup> groups = permissionGroupService.Get();
            bool adminexists = false;

            foreach (PermissionGroup group in groups) {
                if (group.Name == "Admin") {
                    adminexists = true;
                    break;
                }
            }

            if (!adminexists) {
                permissionGroupService.CreateOrModifyPermissionGroupByIdAsync(GetAdminPermissions(mapper));
            }
        }

        public static PermissionGroup GetAdminPermissions(IMapper mapper) {
            PermissionGroupDto admin = new PermissionGroupDto();
            admin.Name = "Admin";
            admin.id = 0;
            for (int i = 0; i < admin.Permissions.Count(); i++)
            {
                admin.Permissions[i].IsEnabled = true;
            }

            return mapper.Map<PermissionGroup>(admin);
        }



    }
}