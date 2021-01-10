using Microsoft.Extensions.Configuration;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IUserCheckDateService : IUserCheckDateRepository
    {
        Task CheckAllUsersAsync();
    }

    public class UserCheckDateService : UserCheckDateRepository, IUserCheckDateService
    {
        private const string DELETED_USER = "Gelöschter User: ";
        private IUserService userService;
        private IModificationEntryService modificationEntryService;
        private IConfiguration configuration;


        public UserCheckDateService(CrmContext context, IUserService userService, IModificationEntryService modificationEntryService, IConfiguration configuration) : base(context)
        {
            this.userService = userService;
            this.modificationEntryService = modificationEntryService;
            this.configuration = configuration;
        }

        /// <summary>
        /// Prüfroutine um zu checken ob der letzte Login-Zeitpunkt eines Users länger als ein konfigurierbarer Zeitraum in der Vergangenheit liegt
        /// </summary>
        /// <returns></returns>
        public async Task CheckAllUsersAsync()
        {

            TimeSpan inactiveSince = TimeSpan.FromDays(1095);
            TimeSpan.TryParse(configuration["DeleteInactiveUsers:InactiveSince"], out inactiveSince);

            List<User> allUsers = await userService.GetAllUsersAsync();
            foreach (User user in allUsers)
            {
                if (user.Id != 1 && !user.IsDeleted && user.LastLoginDate.AddDays(inactiveSince.TotalDays) < DateTime.Now)
                {
                    await modificationEntryService.RemoveUserForeignKeys(user);
                    await userService.DeleteUserAsync(user);
                }
            }
        }
    }
}
