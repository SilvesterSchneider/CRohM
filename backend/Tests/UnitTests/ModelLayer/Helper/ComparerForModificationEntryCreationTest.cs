using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests.ModelLayer.Helper
{
    public class ComparerForModificationEntryCreationTest
    {
        [Fact]
        public void CompareEventsWorksCorrectly()
        {
            // Arrange
            Event oldEvent = new Event() { Id = 1, Date = DateTime.Now, Time = DateTime.Now, Name="testEvent1", Duration = 2.5f };
            EventDto newEvent = new EventDto() { Id = oldEvent.Id, Date = oldEvent.Date, Time = oldEvent.Time, Name = "testEvent2", Duration = 3f };

            // Act
            ComparerForModificationEntryCreation.CompareEvents(oldEvent, newEvent, "ram", out List<ModificationEntry> listWithNewEntries, out List<ModificationEntry> listWithDeletion);

            //Assert
            Assert.Equal(2, listWithNewEntries.Count);
            Assert.Equal(1, listWithNewEntries[0].DataModelId);
            Assert.Equal(MODEL_TYPE.EVENT, listWithNewEntries[0].DataModelType);
            Assert.Equal(DATA_TYPE.NAME, listWithNewEntries[0].DataType);
            Assert.Equal("testEvent2", listWithNewEntries[0].ActualValue);

            Assert.Equal(1, listWithNewEntries[1].DataModelId);
            Assert.Equal(MODEL_TYPE.EVENT, listWithNewEntries[1].DataModelType);
            Assert.Equal(DATA_TYPE.DURATION, listWithNewEntries[1].DataType);
            Assert.Equal("3", listWithNewEntries[1].ActualValue);
        }

        [Fact]
        public void CompareContactsWorksCorrectly()
        {
            // Arrange
            Contact oldContact = new Contact() { Id = 1, Name = "kontakt1", Address = new Address() { City = "Nürnberg" }, ContactPossibilities =
                new ContactPossibilities() { Mail = "info@aa.de" }
            };
            Contact newContact = new Contact() { Id = oldContact.Id, Name = "kontakt2",
                Address = new Address() { City = "Tübingen" }
            };

            // Act
            ComparerForModificationEntryCreation.CompareContacts(oldContact, newContact, "ram", true, out List<ModificationEntry> listWithNewEntries, out List<ModificationEntry> listWithDeletion, 0);

            //Assert
            Assert.Equal(3, listWithNewEntries.Count);
            Assert.Single(listWithDeletion);

            Assert.Equal(1, listWithNewEntries[0].DataModelId);
            Assert.Equal(MODEL_TYPE.CONTACT, listWithNewEntries[0].DataModelType);
            Assert.Equal(DATA_TYPE.NAME, listWithNewEntries[0].DataType);
            Assert.Equal("kontakt2", listWithNewEntries[0].ActualValue);

            Assert.Equal(1, listWithNewEntries[1].DataModelId);
            Assert.Equal(MODEL_TYPE.CONTACT, listWithNewEntries[1].DataModelType);
            Assert.Equal(DATA_TYPE.CITY, listWithNewEntries[1].DataType);
            Assert.Equal("Tübingen", listWithNewEntries[1].ActualValue);

            Assert.Equal(1, listWithDeletion[0].DataModelId);
            Assert.Equal(MODEL_TYPE.CONTACT, listWithDeletion[0].DataModelType);
            Assert.Equal(DATA_TYPE.MAIL, listWithDeletion[0].DataType);
            Assert.True(listWithDeletion[0].IsDeleted);
        }

        [Fact]
        public void CompareOrganizationsWorksCorrectly()
        {
            // Arrange
            Organization oldOrga = new Organization() { Id = 1, Name = "orga1", Address = new Address() { City = "Nürnberg" }, Contact = new ContactPossibilities()
                { PhoneNumber = "1222" }  };
            Organization newOrga = new Organization() { Id = oldOrga.Id,
                Name = "orga2",
                Address = new Address() { City = "Tübingen" }
            };

            // Act
            ComparerForModificationEntryCreation.CompareOrganizations(oldOrga, newOrga, "ram", true, out List<ModificationEntry> listWithNewEntries, out List<ModificationEntry> listWithDeletion, 0);

            //Assert
            Assert.Equal(3, listWithNewEntries.Count);
            Assert.Single(listWithDeletion);

            Assert.Equal(1, listWithNewEntries[0].DataModelId);
            Assert.Equal(MODEL_TYPE.ORGANIZATION, listWithNewEntries[0].DataModelType);
            Assert.Equal(DATA_TYPE.NAME, listWithNewEntries[0].DataType);
            Assert.Equal("orga2", listWithNewEntries[0].ActualValue);

            Assert.Equal(1, listWithNewEntries[1].DataModelId);
            Assert.Equal(MODEL_TYPE.ORGANIZATION, listWithNewEntries[1].DataModelType);
            Assert.Equal(DATA_TYPE.CITY, listWithNewEntries[1].DataType);
            Assert.Equal("Tübingen", listWithNewEntries[1].ActualValue);

            Assert.Equal(1, listWithDeletion[0].DataModelId);
            Assert.Equal(MODEL_TYPE.ORGANIZATION, listWithDeletion[0].DataModelType);
            Assert.Equal(DATA_TYPE.PHONE, listWithDeletion[0].DataType);
            Assert.True(listWithDeletion[0].IsDeleted);
        }
    }
}
