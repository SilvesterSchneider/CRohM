using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer
{ 
    public interface IPermissionGroupService : IPermissionGroupRepositiory {
        Task<List<PermissionGroup>> GetAllPermissionGroupAsync();
        Task CreateOrModifyPermissionGroupByIdAsync(PermissionGroup permissionGroup);
        Task DeleteAsyncWithAllDependencies(long id);
    }

    public class PermissionGroupService : PermissionGroupRepositiory, IPermissionGroupService
    {


        public PermissionGroupService(CrmContext context): base(context) {}


        public async Task<List<PermissionGroup>> GetAllPermissionGroupAsync()
        {
            return await Entities.Include(x => x.Permissions).ToListAsync();
        }

      
        public async Task CreateOrModifyPermissionGroupByIdAsync(PermissionGroup permissionGroup)
        {
            if (permissionGroup.Id == 0 || !(await SetPermissionGroupByIdAsync(permissionGroup))) {
                PermissionGroup result = await CreateAsync(permissionGroup);
            }
        }

        public async Task DeleteAsyncWithAllDependencies(long id)
        {
            PermissionGroup group = await Entities.Include(x => x.Permissions).FirstOrDefaultAsync(y => y.Id == id);
            if (group != null && group.Id != 1)
            {
                await DeleteAsync(group);
            }
        }
    } 
}
