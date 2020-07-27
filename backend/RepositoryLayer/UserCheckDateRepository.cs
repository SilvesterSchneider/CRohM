using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    public interface IUserCheckDateRepository : IBaseRepository<UserDeletionCheckDate>
    {
        DateTime GetTheDateTime();
        Task UpdateAsync(DateTime dateTime);
    }

    public class UserCheckDateRepository : BaseRepository<UserDeletionCheckDate>, IUserCheckDateRepository
    {
        public UserCheckDateRepository(CrmContext context) : base(context)
        {

        }

        public void CheckAllTheUsers()
        {
            throw new NotImplementedException();
        }

        public DateTime GetTheDateTime()
        {
            if (Entities.ToList().Count >= 0)
            {
                return Entities.ToList()[0].DateOfLastCheck;
            }
            else
            {
                return DateTime.Now;
            }
        }

        public async Task UpdateAsync(DateTime dateTime)
        {
            if (Entities.ToList().Count >= 1)
            {
                UserDeletionCheckDate checkDate = Entities.ToList()[0];
                checkDate.DateOfLastCheck = dateTime;
                await UpdateAsync(checkDate);
            }
            else
            {
                UserDeletionCheckDate checkDate = new UserDeletionCheckDate() { DateOfLastCheck = dateTime };
                await CreateAsync(checkDate);
            }
        }
    }
}
