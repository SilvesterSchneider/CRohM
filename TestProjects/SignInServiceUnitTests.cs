using Microsoft.Extensions.Options;
using ModelLayer.Models;
using Moq;
using ServiceLayer;
using WebApi.Helper;
using Xunit;

namespace TestProjects
{
    public class SignInServiceUnitTests
    {
        [Fact]
        public void CreateToken_WorksCorrectly()
        {
            //Arrange
            var mockManager = new Mock<ISignInManager>();
            IOptions<AppSettings> mockAppSettings = Options.Create<AppSettings>(new AppSettings() { JwtSecret = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" });

            var sut = new SignInService(mockAppSettings, mockManager.Object);

            //Act
            var token = sut.CreateToken(new User());

            //Assert
            Assert.False(string.IsNullOrEmpty(token));
        }
    }
}