using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using ModelLayer.Models;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IModificationEntryService : IModificationEntryRepository
    {
        Task UpdateContactAsync(string usernameOfModification, Contact oldContact, Contact newContact, bool deleteEntries);
        Task UpdateOrganizationAsync(string usernameOfModification, Organization oldOrga, Organization newOrga, bool deleteEntries);
        Task UpdateEventsAsync(string usernameOfModification, Event oldEvent, EventDto newEvent);
        Task CreateNewContactEntryAsync(string userNameOfChange, long id);
        Task CreateNewOrganizationEntryAsync(string userNameOfChange, long id);
        Task CreateNewEventEntryAsync(string userNameOfChange, long id);
        Task UpdateContactByHistoryElementAsync(string userNameOfChange, long id);
        Task UpdateOrganizationByHistoryElementAsync(string userNameOfChange, long id);
        Task UpdateContactByDeletionAsync(long id);
        Task UpdateOrganizationByDeletionAsync(long id);
        Task UpdateEventByDeletionAsync(long id);
        Task CommitChanges();
        Task ChangeEmployeesOfOrganization(long id, string contactName, bool wasDeleted, string userNameOfChange);
        Task ChangeContactsOfEvent(long id, int oldCount, int newCount, string userNameOfChange);
    }

    public class ModificationEntryService : ModificationEntryRepository, IModificationEntryService
    {
        private List<ModificationEntry> listWithCreation;
        private List<ModificationEntry> listWithDeletion;

        public ModificationEntryService(CrmContext context) : base(context)
        {

        }

        public async Task CreateNewContactEntryAsync(string userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.CREATED, MODEL_TYPE.CONTACT, DATA_TYPE.NONE);
        }

        public async Task UpdateContactAsync(string usernameOfModification, Contact oldContact, Contact newContact, bool deleteEntries)
        {
            await Task.Run(() => ComparerForModificationEntryCreation.CompareContacts(oldContact, newContact, usernameOfModification, deleteEntries, out listWithCreation, out listWithDeletion));            
        }

        public async Task CommitChanges()
        {
            foreach (ModificationEntry entry in listWithDeletion)
            {
                await SetEntriesToDeletionStateAsync(entry.DataModelId, entry.DataModelType, entry.DataType, entry.ExtensionIndex);
            }
            foreach (ModificationEntry entry in listWithCreation)
            {
                await CreateAsync(entry);
            }
        }

        public async Task UpdateContactByDeletionAsync(long id)
        {
            await CreateNewEntryAsync("", id, MODIFICATION.DELETED, MODEL_TYPE.CONTACT, DATA_TYPE.NONE);
        }

        public async Task UpdateContactByHistoryElementAsync(string userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.MODIFIED, MODEL_TYPE.CONTACT, DATA_TYPE.HISTORY_ELEMENT);
        }

        public async Task UpdateOrganizationAsync(string usernameOfModification, Organization oldOrga, Organization newOrga, bool deleteEntries)
        {
            await Task.Run(() => ComparerForModificationEntryCreation.CompareOrganizations(oldOrga, newOrga, usernameOfModification, deleteEntries, out listWithCreation, out listWithDeletion));
        }

        public async Task CreateNewOrganizationEntryAsync(string userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.CREATED, MODEL_TYPE.ORGANIZATION, DATA_TYPE.NONE);
        }

        public async Task UpdateOrganizationByHistoryElementAsync(string userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.ADDED, MODEL_TYPE.ORGANIZATION, DATA_TYPE.HISTORY_ELEMENT);
        }

        public async Task UpdateOrganizationByDeletionAsync(long id)
        {
            await CreateNewEntryAsync("", id, MODIFICATION.DELETED, MODEL_TYPE.ORGANIZATION, DATA_TYPE.NONE);
        }

        public async Task ChangeEmployeesOfOrganization(long id, string contactName, bool wasDeleted, string userNameOfChange)
        {
            if (wasDeleted)
            {
                await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.DELETED, MODEL_TYPE.ORGANIZATION, DATA_TYPE.CONTACTS, contactName, "");
            }
            else
            {
                await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.ADDED, MODEL_TYPE.ORGANIZATION, DATA_TYPE.CONTACTS, "", contactName);
            }         
        }

        public async Task UpdateEventsAsync(string usernameOfModification, Event oldEvent, EventDto newEvent)
        {
            await Task.Run(() => ComparerForModificationEntryCreation.CompareEvents(oldEvent, newEvent, usernameOfModification, out listWithCreation, out listWithDeletion));
        }

        public async Task CreateNewEventEntryAsync(string userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.CREATED, MODEL_TYPE.EVENT, DATA_TYPE.NONE);
        }

        public async Task UpdateEventByDeletionAsync(long id)
        {
            await CreateNewEntryAsync("", id, MODIFICATION.DELETED, MODEL_TYPE.EVENT, DATA_TYPE.NONE);
        }

        public async Task ChangeContactsOfEvent(long id, int oldCount, int newCount, string userNameOfChange)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.MODIFIED, MODEL_TYPE.EVENT, DATA_TYPE.CONTACTS, oldCount.ToString(), newCount.ToString());
        }
    }
}
