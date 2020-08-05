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

        /// <summary>
        /// Den Zeitstempel holen um zu prüfen ob die Überprüfung aller Users stattfinden soll
        /// </summary>
        /// <returns></returns>
        public DateTime GetTheDateTime()
        {
            if (Entities.ToList().Any())
            {
                return Entities.ToList().ElementAt(0).DateOfLastCheck;
            }
            else
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Den Zeitstempel in der DB aktualisieren
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
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
