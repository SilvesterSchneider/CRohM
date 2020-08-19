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

        public UserCheckDateService(CrmContext context, IUserService userService, IModificationEntryService modificationEntryService) : base(context)
        {
            this.userService = userService;
            this.modificationEntryService = modificationEntryService;
        }

        /// <summary>
        /// Prüfroutine um zu checken ob der letzte Login-Zeitpunkt eines Users länger als 3 Jahre in der Vergangenheit liegt
        /// </summary>
        /// <returns></returns>
        public async Task CheckAllUsersAsync()
        {
            List<User> allUsers = await userService.GetAllUsersAsync();
            foreach (User user in allUsers)
            {
                if (user.Id != 1 && !user.IsDeleted && user.LastLoginDate.AddYears(3) < DateTime.Now)
                {

                    await modificationEntryService.RemoveUserForeignKeys(user);
                    await userService.DeleteUserAsync(user);

                }
            }
        }
    }
}
