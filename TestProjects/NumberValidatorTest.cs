using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestProjects
{
    public class NumberValidatorTest
    {
        [Fact]
        public void IsValid_Correct_Data_AssertTrue()
        {
            NumberValidator val = new NumberValidator();
            Assert.True(val.IsValid("0172-9483597"));
        }

        [Fact]
        public void IsValid_Wrong_Data_AssertFalse()
        {
            NumberValidator val = new NumberValidator();
            Assert.False(val.IsValid("0s7294835a7"));
        }
    }
}
