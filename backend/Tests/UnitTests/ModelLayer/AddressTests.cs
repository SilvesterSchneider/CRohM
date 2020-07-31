using ModelLayer.Models;
using Xunit;

namespace UnitTests.ModelLayer
{
    public class AddressTests
    {
        [Fact]
        public void Address_Create()
        {
            //Arrange
            Address sut;

            //Act
            sut = new Address();

            //Assert
            Assert.Equal("", sut.City);// assume that rest of strings have same behavior
        }
    }
}
