using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using ModelLayer.Models;
using Moq;
using ServiceLayer;
using Xunit;

namespace UnitTests.ServiceLayer
{
    public class UserServiceTests
    {
        [Fact]
        public void CreateCRohMUserAsync_WorksCorrectly()
        {
            // Arrange
            var mockMailProvider = new Mock<IMailProvider>();
            var mockUserManager = new Mock<IUserManager>();
            mockUserManager.Setup(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var sut = new Mock<UserService>(mockUserManager.Object, mockMailProvider.Object);
            sut.Setup(service => service.GetUniqueUserNameAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("wurstha12");

            //Act
            var testUser = new User() { FirstName = "hans", LastName = "wurst", Email = "hans@wurst.com" };
            var result = sut.Object.CreateCRohMUserAsync(testUser).Result;

            //Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void GetUniqueUserNameAsync_WorksCorrectly()
        {
            // Arrange
            var mockUserManager = new Mock<IUserManager>();
            var mockMailProvider = new Mock<IMailProvider>();
            var sut = new UserService(mockUserManager.Object, mockMailProvider.Object);

            var users = new List<User>()
            {
                new User(),
                new User()
            };

            var mock = users.AsQueryable().BuildMock();

            mockUserManager.Setup(manager => manager.Users)
                .Returns(mock.Object);

            //Act
            var userName = sut.GetUniqueUserNameAsync("hans", "wurst").Result;

            //Assert
            Assert.Equal("wurstha21", userName);
        }
    }
}