using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ModelLayer.Helper;

namespace RepositoryLayer
{
    public interface IUserLoginRepository : IBaseRepository<UserLogin>
    {
        Task<DateTime> GetTheLastLoginDateTimeForUserByIdAsync(long id);
    }

    public class UserLoginRepository : BaseRepository<UserLogin>, IUserLoginRepository
    {
        public UserLoginRepository(CrmContext context) : base(context)
        {

        }

        public async Task<DateTime> GetTheLastLoginDateTimeForUserByIdAsync(long id)
        {
            List<UserLogin> userLogins = await Entities.Where(a => a.UserId == id).ToListAsync();
            if (userLogins.Count > 1)
            {
                return userLogins[userLogins.Count - 2].DateTimeOfLastLogin;
            }
            else if (userLogins.Count == 1)
            {
                return userLogins[0].DateTimeOfLastLogin;
            }
            else
            {
                return DateTime.Now;
            }
        }
    }
}
