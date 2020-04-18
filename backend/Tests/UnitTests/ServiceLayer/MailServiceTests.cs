using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests.ServiceLayer
{
    public class MailServiceTests
    {
        [Fact]
        public void Registration_CorrectData_AssertPassed()
        {
            //Arrage
            string user = "Test";
            string password = "password";
            string correctemailaddress = "crohm_nuernberg@hotmail.com";
            MailService ms = new MailService();

            //Act
            bool erg = ms.Registration(user, password, correctemailaddress);

            //Assert
            Assert.True(erg);
        }

        [Fact]
        public void Registration_FalseData_AssertFailed()
        {
            // Arrage
            string user = "Test";
            string password = "password";
            string falsemailaddress = "bla bla";
            MailService ms = new MailService();

            //Act
            bool erg = ms.Registration(user, password, falsemailaddress);

            //Assert
            Assert.False(erg);
        }

        [Fact]
        public void Passwordreset_FalseData_AssertPassed()
        {
            // Arrage
            string password = "password";
            string falsemailaddress = "crohm_nuernberg@hotmail.com";
            MailService ms = new MailService();

            //Act
            bool erg = ms.PasswordReset(password, falsemailaddress);

            //Assert
            Assert.True(erg);
        }

        [Fact]
        public void Passwordreset_FalseData_AssertFailed()
        {
            // Arrage
            string password = "password";
            string falsemailaddress = "blabla";
            MailService ms = new MailService();

            //Act
            bool erg = ms.PasswordReset(password, falsemailaddress);

            //Assert
            Assert.False(erg);
        }
    }
}