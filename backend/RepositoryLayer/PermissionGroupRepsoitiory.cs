using ModelLayer;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer
{ 
    public interface IPermissionGroupRepositiory : IBaseRepository<PermissionGroup>
    {
        Task<PermissionGroup> GetPermissionGroupByIdAsync(long id);
        Task<bool> SetPermissionGroupByIdAsync(PermissionGroup permissiongGroup);
    }

    public class PermissionGroupRepositiory : BaseRepository<PermissionGroup>, IPermissionGroupRepositiory
    {
        public PermissionGroupRepositiory(CrmContext context) : base(context) { }

        public async Task<PermissionGroup> GetPermissionGroupByIdAsync(long id)
        {
            return await Entities.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> SetPermissionGroupByIdAsync(PermissionGroup permissiongGroup)
        {
            PermissionGroup groupToModify = await Entities.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == permissiongGroup.Id);
            if (groupToModify != null) {
                groupToModify.Permissions.Clear();
                groupToModify.Permissions.AddRange(permissiongGroup.Permissions);
                await UpdateAsync(groupToModify);
                return true;
            }
            return false;
        }
    } 
}
