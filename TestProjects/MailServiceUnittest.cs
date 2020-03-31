using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestProjects
{
    public class MailServiceUnittest
    {
        private readonly string PASSWORD_TEST = "password";
        private readonly string EMAIL_TEST = "hans@maier.de";

        [Fact]
        public void Registration_CorrectData_AssertPassed()
        {
            Assert.True(new MailService().PasswordReset(PASSWORD_TEST, EMAIL_TEST));
        }
    }
}
