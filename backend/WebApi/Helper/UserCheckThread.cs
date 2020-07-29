using ServiceLayer;
using System;
using System.Threading;

namespace WebApi.Helper
{
    public class UserCheckThread 
    {
        private static DateTime DEF_TIME = DateTime.MinValue;
        private DateTime dateTime = DEF_TIME;
        private IUserCheckDateService userCheckSevice;

        public UserCheckThread(IUserCheckDateService userCheckSevice)
        {
            this.userCheckSevice = userCheckSevice;
        }

        public void runThread()
        {
            new Thread(new ThreadStart(CheckAction)).Start();
        }

        private void CheckAction()
        {
            while (true)
            {
                if (dateTime.Equals(DEF_TIME))
                {
                    dateTime = userCheckSevice.GetTheDateTime();
                }
                if (DateTime.Now > dateTime)
                {
                    dateTime = DateTime.Now.AddDays(1);
                    userCheckSevice.UpdateAsync(dateTime).Wait();
                    userCheckSevice.CheckAllUsersAsync();
                }
                Thread.Sleep(1000 * 60 * 60);
            }
        }
    }
}
