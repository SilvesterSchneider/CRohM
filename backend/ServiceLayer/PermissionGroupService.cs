using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ModelLayer.Helper;

namespace ServiceLayer
{
    public interface IPermissionGroupService : IPermissionGroupRepositiory
    {
        Task<List<PermissionGroup>> GetAllPermissionGroupAsync();

        Task CreatePermissionGroupAsync(PermissionGroup permissionGroup);

        Task UpdatePermissionGroupByIdAsync(PermissionGroup permissionGroup);

        Task DeleteAsyncWithAllDependencies(long id);

        Task<bool> IsThereAnyDataProtectionOfficerAsync();
    }

    public class PermissionGroupService : PermissionGroupRepository, IPermissionGroupService
    {
        private RoleManager<Permission> _roleManager;

        public PermissionGroupService(CrmContext context, RoleManager<Permission> roleManager) : base(context)
        {
            _roleManager = roleManager;
        }

        public async Task<List<PermissionGroup>> GetAllPermissionGroupAsync()
        {
            return await Entities.Include(x => x.Permissions).ToListAsync();
        }

        public async Task CreatePermissionGroupAsync(PermissionGroup permissionGroup)
        {
            foreach (PermissionGroup group in Entities)
            {
                if (group.Name.Equals(permissionGroup.Name))
                {
                    return;
                }
            }
            PermissionGroup newPerm = new PermissionGroup();
            newPerm.Id = 0;
            newPerm.Name = permissionGroup.Name;
            foreach (Permission perm in AllRoles.GetAllRoles())
            {
                foreach (Permission permNew in permissionGroup.Permissions)
                {
                    if (perm.Name == permNew.Name)
                    {
                        perm.Id = 0;
                        newPerm.Permissions.Add(perm);
                    }
                }
            }
            await CreateAsync(newPerm);
        }

        public async Task UpdatePermissionGroupByIdAsync(PermissionGroup permissionGroup)
        {
            PermissionGroup groupToModify = await Entities.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == permissionGroup.Id);
            if (groupToModify != null)
            {
                groupToModify.Name = permissionGroup.Name;
                List<Permission> newPermissions = new List<Permission>();
                foreach (Permission r in permissionGroup.Permissions)
                {
                    Permission role = AllRoles.GetAllRoles().FirstOrDefault(x => x.Name == r.Name);
                    if (role != null)
                    {
                        Permission alreadyExistingOne = groupToModify.Permissions.FirstOrDefault(x => x.Name == r.Name);
                        if (alreadyExistingOne != null)
                        {
                            newPermissions.Add(alreadyExistingOne);
                        }
                        else
                        {
                            role.Id = 0;
                            newPermissions.Add(role);
                        }
                    }
                }
                groupToModify.Permissions = newPermissions;

                await SetPermissionGroupByIdAsync(groupToModify);
            }
        }

        public async Task DeleteAsyncWithAllDependencies(long id)
        {
            //Checken ob noch ein User die PG hat!!!
            PermissionGroup group = await Entities.Include(x => x.Permissions).FirstOrDefaultAsync(y => y.Id == id);
            if (group != null && group.Id != 1)
            {
                await DeleteAsync(group);
            }
        }

        public async Task<bool> IsThereAnyDataProtectionOfficerAsync()
        {
            return await Entities
                .Include(group => group.User)
                .ThenInclude(upg => upg.User)
                .Where(group => group.Name == "Datenschutzbeauftragter")
                .AnyAsync(group => group.User.Any());
        }
    }
}
