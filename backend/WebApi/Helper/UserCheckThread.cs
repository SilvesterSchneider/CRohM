using RepositoryLayer;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebApi.Helper
{
    public class UserCheckThread 
    {
        private DateTime dateTime;
        private IUserCheckDateService<UserCheckThread> userCheckRepo;

        public UserCheckThread(IUserCheckDateService userCheckSevice)
        {
            this.userCheckRepo = userCheckSevice;
            new Thread(new ThreadStart(CheckAction)).Start();
        }

        private void CheckAction()
        {
            while (true)
            {
                if (dateTime == null)
                {
                    dateTime = userCheckRepo.GetTheDateTime();
                }
                if (DateTime.Now > dateTime)
                {
                    dateTime = dateTime.AddDays(1);
                    userCheckRepo.UpdateAsync(dateTime).Wait();
                    userCheckRepo.CheckAllUsersAsync();
                }
                Thread.Sleep(1000 * 60 * 60);
            }
        }
    }
}
