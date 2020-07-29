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
        private IUserCheckDateService userCheckSevice;

        public UserCheckThread(IUserCheckDateService userCheckSevice)
        {
            this.userCheckSevice = userCheckSevice;
            new Thread(new ThreadStart(CheckAction)).Start();
        }

        private void CheckAction()
        {
            while (true)
            {
                if (dateTime == null)
                {
                    dateTime = userCheckSevice.GetTheDateTime();
                }
                if (DateTime.Now > dateTime)
                {
                    dateTime = dateTime.AddDays(1);
                    userCheckSevice.UpdateAsync(dateTime).Wait();
                    userCheckSevice.CheckAllUsersAsync();
                }
                Thread.Sleep(1000 * 60 * 60);
            }
        }
    }
}
