using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using Newtonsoft.Json;
using ServiceLayer;
using WebApi;
using Xunit;

namespace ServiceLayerTests.Services
{
    public class AlarmServiceTest : BaseServiceTest
    {
        public AlarmServiceTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async void NameOfTestFunction_WhatWillBeTested_WhatWillBeAsserted()
        {
            //TODO: implement example from https://code-maze.com/integration-testing-asp-net-core-mvc/
            //Arrange
            //Act

            var httpResponse = await Client.GetAsync("/api/addresses");

            httpResponse.EnsureSuccessStatusCode();

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var addressDtos = JsonConvert.DeserializeObject<IEnumerable<AddressDto>>(stringResponse);

            //Assert
            Assert.Single(addressDtos);
        }
    }
}