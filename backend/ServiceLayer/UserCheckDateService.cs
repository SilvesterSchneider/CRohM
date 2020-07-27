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
        private IUserService userService;

        public UserCheckDateService(CrmContext context, IUserService userService) : base(context)
        {
            this.userService = userService;
        }

        public async Task CheckAllUsersAsync()
        {
            List<User> allUsers = await userService.GetAllUsersAsync();
            foreach (User user in allUsers)
            {
                if (user.LastLoginDate.AddYears(3) < DateTime.Now)
                {
                    await userService.DeleteUserAsync(user);
                }
            }
        }
    }
}
