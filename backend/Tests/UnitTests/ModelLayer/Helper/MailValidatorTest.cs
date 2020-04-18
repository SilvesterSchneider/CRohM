using ModelLayer.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests.ModelLayer.Helper
{
    public class MailValidatorTest
    {
        [Fact]
        public void IsValid_Correct_Data_AssertTrue()
        {
            MailValidator val = new MailValidator();
            Assert.True(val.IsValid("r.matis@gmail.com"));
        }

        [Fact]
        public void IsValid_Wrong_Data_AssertFalse()
        {
            NumberValidator val = new NumberValidator();
            Assert.False(val.IsValid("rm.matis.com"));
        }
    }
}