using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    public interface IUserPermissionGroupRepository : IBaseRepository<UserPermissionGroup>
    {
        Task<UserPermissionGroup> GetUserPermissionGroupByIdAsync(long groupId, long userId);
    }

    public class UserPermissionGroupRepository : BaseRepository<UserPermissionGroup>, IUserPermissionGroupRepository
    {
        public UserPermissionGroupRepository(CrmContext context) : base(context)
        {

        }

        public async Task<UserPermissionGroup> GetUserPermissionGroupByIdAsync(long groupId, long userId)
        {
            return await Entities.Include(x => x.PermissionGroup).Include(y => y.User).FirstOrDefaultAsync(x => x.UserId == userId && x.PermissionGroupId == groupId);
        }
    }
}
