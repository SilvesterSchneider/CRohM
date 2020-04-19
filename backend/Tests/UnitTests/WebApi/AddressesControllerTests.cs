using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using Moq;
using ServiceLayer;
using WebApi.Controllers;
using Xunit;

namespace UnitTests.WebApi
{
    public class AddressesControllerTests
    {
        [Fact]
        public async void Get_ReturnsOk()//no need every time to write a scenario
        {
            //Arrange
            var mockAddressService = new Mock<IAddressService>();
            mockAddressService.Setup(service => service.GetAsync())
                .ReturnsAsync(() => new List<Address>());
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<List<AddressDto>>(It.IsAny<List<Address>>()))
                .Returns(() => new List<AddressDto>());

            var sut = new AddressesController(mockAddressService.Object, mockMapper.Object);

            //Act
            var result = await sut.Get();

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}