using ModelLayer;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using ModelLayer.Helper;
using Microsoft.AspNetCore.Identity;

namespace RepositoryLayer
{ 
    public interface IPermissionGroupRepositiory : IBaseRepository<PermissionGroup>
    {
        Task<PermissionGroup> GetPermissionGroupByIdAsync(long id);
        Task<PermissionGroup> SetPermissionGroupByIdAsync(PermissionGroup permissiongGroup);
    }

    public class PermissionGroupRepository : BaseRepository<PermissionGroup>, IPermissionGroupRepositiory
    {
        public PermissionGroupRepository(CrmContext context) : base(context) { }

 
        public async Task<PermissionGroup> GetPermissionGroupByIdAsync(long id)
        {
            return await Entities.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PermissionGroup> SetPermissionGroupByIdAsync(PermissionGroup permissionGroup)
        {
               return await UpdateAsync(permissionGroup);
        }

        public async Task<PermissionGroup> CreateCheckedAsync(PermissionGroup permissionGroup)
        {
            return await CreateAsync(permissionGroup);
        }
    } 
}
