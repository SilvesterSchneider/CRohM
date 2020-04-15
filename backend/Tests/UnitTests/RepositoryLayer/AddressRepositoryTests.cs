using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using Xunit;

namespace UnitTests.RepositoryLayer
{
    public class AddressRepositoryTests
    {
        [Fact]
        public async void GetByZipcodeAsync_SearchWithExistingZipcode_ReturnsSingleAddress()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<CrmContext>()
                .UseInMemoryDatabase(databaseName: "AddressesDatabase")
                .Options;

            using (var context = new CrmContext(options))
            {
                context.Addresses.Add(new Address() { Zipcode = "12345" });
                context.Addresses.Add(new Address() { Zipcode = "54321" });
                context.Addresses.Add(new Address() { Zipcode = "11111" });
                context.SaveChanges();
            }

            var zipcodeToSearch = "12345";
            List<Address> result;

            //Act
            using (var context = new CrmContext(options))
            {
                AddressRepository sut = new AddressRepository(context);
                result = await sut.GetByZipcodeAsync(zipcodeToSearch);
            }

            //Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(zipcodeToSearch, result.First().Zipcode);
        }
    }
}