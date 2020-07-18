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

namespace WebApi.Helper
{
    public static class ApplicationDbInitializer
    {
        /// <summary>
        /// Seeding database with admin user and password admin. Also add role 'Admin'
        /// </summary>
        /// <param name="userService">Service to include user</param>
        public static void SeedUsers(IUserService userService, IPermissionGroupService permissionService, IUserPermissionGroupRepository userPermissionGroupRepo)
        {
            if (userService.FindByEmailAsync("admin@admin.com").Result == null)
            {
                User user = new User
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    LastName = "admin"
                };
                userService.CreateAsync(user, "@dm1n1stR4tOr").Wait();
                User admin = userService.GetByIdAsync(1).Result;
                PermissionGroup permissionGroup = permissionService.GetPermissionGroupByIdAsync(1).Result;
                UserPermissionGroup connection = new UserPermissionGroup() { User=admin, UserId=1, PermissionGroup=permissionGroup, PermissionGroupId=1 };
                userPermissionGroupRepo.CreateAsync(connection).Wait();
                UserPermissionGroup result = userPermissionGroupRepo.GetUserPermissionGroupByIdAsync(1, 1).Result;
                admin.Permission.Add(result);
                userService.UpdateAsync(admin).Wait();
            }
        }

        public static void SeedPermissions(IPermissionGroupService permissionGroupService, IMapper mapper) {

            List<PermissionGroup> groups = permissionGroupService.GetAllPermissionGroupAsync().Result;
            PermissionGroup adminGroup = null;
            foreach (PermissionGroup group in groups) {
                if (group.Id == 1) {
                    adminGroup = group;
                    break;
                }
            }

            if (adminGroup == null) {
                permissionGroupService.CreatePermissionGroupAsync(GetAdminPermissions()).Wait();
            }
            else
            {
                bool allPermissionsAvailable = true;
                bool namingOk = true;
                List<Permission> actualPermissions = AllRoles.GetAllRoles();
                foreach (Permission permToCheck in actualPermissions)
                {
                    Permission permissionToCheck = adminGroup.Permissions.FirstOrDefault(a => a.UserRight == permToCheck.UserRight);
                    if (permissionToCheck == null)                        
                    {
                        allPermissionsAvailable = false;
                    }
                    if (permissionToCheck != null && !permissionToCheck.Name.Equals(permToCheck.Name))
                    {
                        namingOk = false;
                    }
                }
                if (actualPermissions.Count != adminGroup.Permissions.Count)
                {
                    allPermissionsAvailable = false;
                }
                if (!allPermissionsAvailable || !namingOk)
                {
                    if (!allPermissionsAvailable)
                    {
                        adminGroup.Permissions = actualPermissions;
                        permissionGroupService.UpdatePermissionGroupByIdAsync(adminGroup).Wait();
                    }
                    if (!namingOk)
                    {
                        List<PermissionGroup> allGroups = permissionGroupService.GetAllPermissionGroupAsync().Result;
                        foreach (PermissionGroup groupToCheck in allGroups)
                        {
                            List<Permission> permissions = new List<Permission>();
                            foreach (Permission oldPermission in groupToCheck.Permissions)
                            {
                                Permission newPermission = actualPermissions.FirstOrDefault(a => a.UserRight == oldPermission.UserRight);
                                if (newPermission != null)
                                {
                                    permissions.Add(newPermission);
                                }
                            }
                            groupToCheck.Permissions = permissions;
                            permissionGroupService.UpdatePermissionGroupByIdAsync(groupToCheck).Wait();
                        }
                    }
                }
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
