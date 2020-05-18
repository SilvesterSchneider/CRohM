using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    public class LoggedInUser
    {
        private static User loggedInUser;

        public static User GetLoggedInUser()
        {
            return loggedInUser;
        }

        public static void SetLoggedInUser(User user)
        {
            loggedInUser = user;
        }
    }
}
