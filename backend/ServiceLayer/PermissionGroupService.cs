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
    public interface IPermissionGroupService : IPermissionGroupRepositiory {

        Task<List<PermissionGroup>> GetAllPermissionGroupAsync();
        Task CreatePermissionGroupAsync(PermissionGroup permissionGroup);
        Task UpdatePermissionGroupByIdAsync(PermissionGroup permissionGroup);
        Task DeleteAsyncWithAllDependencies(long id);
    }

  

    public class PermissionGroupService : PermissionGroupRepository, IPermissionGroupService
    {
        RoleManager<Permission> _roleManager;
        public PermissionGroupService(CrmContext context, RoleManager<Permission> roleManager): base(context) {
            _roleManager = roleManager;
        }


        public async Task<List<PermissionGroup>> GetAllPermissionGroupAsync()
        {
            return await Entities.Include(x => x.Permissions).ToListAsync();
        }

        
        public async Task CreatePermissionGroupAsync(PermissionGroup permissionGroup) {
            PermissionGroup newPerm = new PermissionGroup();
            newPerm.Name = permissionGroup.Name;
            foreach (Permission perm in AllRoles.GetAllRoles()){
                foreach (Permission permNew in permissionGroup.Permissions) {

                    if (perm.Name == permNew.Name){
                        newPerm.Permissions.Add(perm);                    
                    }
                }
            }

            await CreateCheckedAsync(newPerm);
        }


        public async Task UpdatePermissionGroupByIdAsync(PermissionGroup permissionGroup)
        {
            PermissionGroup groupToModify = await Entities.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == permissionGroup.Id);
            if (groupToModify != null)
            {
                groupToModify.Permissions.Clear();
                foreach (Permission r in permissionGroup.Permissions)
                {
                    Permission role = AllRoles.GetAllRoles().FirstOrDefault(x => x.Name == r.Name);
                    if (role != null)
                    {
                        groupToModify.Permissions.Add(role);
                    }
                }
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
    } 
}
