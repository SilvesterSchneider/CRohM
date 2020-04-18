using ModelLayer.Helper;
using System;
using Xunit;

namespace UnitTests.ModelLayer.Helper
{
    public class PasswordGeneratorTest
    {
        private const int LENGTH_FIRST = 10;
        private const int LENGTH_SECOND = 20;

        [Fact]
        public void TestIfCorrectLengthIsCreated()
        {
            PasswordGenerator password = new PasswordGenerator(LENGTH_FIRST, LENGTH_FIRST, 2, 2, 3, 3);
            string pass = password.Generate();
            Console.WriteLine("Password: " + pass);
            Assert.Equal(LENGTH_FIRST, pass.Length);
            password = new PasswordGenerator(LENGTH_SECOND, LENGTH_SECOND, 4, 5, 3, 3);
            pass = password.Generate();
            Console.WriteLine("Password: " + pass);
            Assert.Equal(LENGTH_SECOND, pass.Length);
        }
    }
}