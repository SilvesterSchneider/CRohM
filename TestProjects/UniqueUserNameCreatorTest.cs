using ModelLayer.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestProjects
{
    public class UniqueUserNameCreatorTest
    {
        private const string FIRSTNAME = "razvan";
        private const string LASTNAME = "matis";
        private const int NUMBER = 81072;
        private const string EXPECTED_USERNAME = "matisra81072";

        [Fact]
        public void TestIfUniqueUserNameIsBeingCreatedCorrectly()
        {
            Assert.Equal(EXPECTED_USERNAME, UniqueNameCreator.CreateUniqueName(FIRSTNAME, LASTNAME, NUMBER));
        }
    }
}
