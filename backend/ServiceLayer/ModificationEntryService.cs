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
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public interface IModificationEntryService : IModificationEntryRepository
    {
        /// <summary>
        /// Update a specific contact.
        /// </summary>
        /// <param name="usernameOfModification">who modified it?</param>
        /// <param name="oldContact">the old contact</param>
        /// <param name="newContact">the new contact</param>
        /// <param name="deleteEntries">should all the entries be really deleted after it was removed?</param>
        /// <returns></returns>
        Task UpdateContactAsync(User usernameOfModification, Contact oldContact, Contact newContact, bool deleteEntries);

        /// <summary>
        /// update a specific organization.
        /// </summary>
        /// <param name="usernameOfModification">who modified it?</param>
        /// <param name="oldOrga">old organization</param>
        /// <param name="newOrga">new organization</param>
        /// <param name="deleteEntries">should all the entries be really deleted after it was removed?</param>
        /// <returns></returns>
        Task UpdateOrganizationAsync(User usernameOfModification, Organization oldOrga, Organization newOrga, bool deleteEntries);

        /// <summary>
        /// Update a event
        /// </summary>
        /// <param name="usernameOfModification">who modified it?</param>
        /// <param name="oldEvent">old event</param>
        /// <param name="newEvent">new event</param>
        /// <returns></returns>
        Task UpdateEventsAsync(User usernameOfModification, Event oldEvent, EventDto newEvent, List<Contact> contactsParticipated, List<Organization> orgasParticipated);

        /// <summary>
        /// creates a new modification entry just for insertion of a new contact
        /// </summary>
        /// <param name="userNameOfChange">who created it?</param>
        /// <param name="id">the id of new model</param>
        /// <returns></returns>
        Task CreateNewContactEntryAsync(User userNameOfChange, long id);

        /// <summary>
        /// creates a new modification entry just for insertion of a new organization
        /// </summary>
        /// <param name="userNameOfChange">who created it?</param>
        /// <param name="id">the id of new model</param>
        /// <returns></returns>
        Task CreateNewOrganizationEntryAsync(User userNameOfChange, long id);

        /// <summary>
        /// creates a new modification entry just for insertion of a new event
        /// </summary>
        /// <param name="userNameOfChange">who created it?</param>
        /// <param name="id">the id of new model</param>
        /// <returns></returns>
        Task CreateNewEventEntryAsync(User userNameOfChange, long id);

        /// <summary>
        /// create a modification entry for the info that a history element was added to a contact
        /// </summary>
        /// <param name="userNameOfChange">who added it?</param>
        /// <param name="id">the id of new model</param>
        /// <param name="historyElementContent">the new content of history element</param>
        /// <returns></returns>
        Task UpdateContactByHistoryElementAsync(User userNameOfChange, long id, string historyElementContent);

        /// <summary>
        /// create a modification entry for the info that a history element was added to a organization
        /// </summary>
        /// <param name="userNameOfChange">who added it?</param>
        /// <param name="id">the id of new model</param>
        /// <param name="historyElementContent">the new content of history element</param>
        /// <returns></returns>
        Task UpdateOrganizationByHistoryElementAsync(User userNameOfChange, long id, string historyElementContent);

        /// <summary>
        /// creates a new modification entry as info that a contact was deleted
        /// </summary>
        /// <param name="id">the id of model</param>
        /// <returns></returns>
        Task UpdateContactByDeletionAsync(long id);

        /// <summary>
        /// creates a new modification entry as info that a organization was deleted
        /// </summary>
        /// <param name="id">the id of model</param>
        /// <returns></returns>
        Task UpdateOrganizationByDeletionAsync(long id);

        /// <summary>
        /// creates a new modification entry as info that an event was deleted
        /// </summary>
        /// <param name="id">the id of model</param>
        /// <returns></returns>
        Task UpdateEventByDeletionAsync(long id);

        /// <summary>
        /// Commit all changes to the database after the update process was finished before. its just to make sure, that the new model
        /// was created sucessfully.
        /// </summary>
        /// <returns></returns>
        Task CommitChanges();

        /// <summary>
        /// create a modification entry for the info that the employees of an organization have changed
        /// </summary>
        /// <param name="id">the id of model</param>
        /// <param name="contactName">the contact to consider</param>
        /// <param name="wasDeleted">if true, then he/she was deleted, otherwise he/she was added</param>
        /// <param name="userNameOfChange">who made this change?</param>
        /// <returns></returns>
        Task ChangeEmployeesOfOrganization(long id, string contactName, bool wasDeleted, User userNameOfChange);

        /// <summary>
        /// create a modification entry for the info that the invited contact list of events have changed
        /// </summary>
        /// <param name="id">the model id</param>
        /// <param name="contactName">the contact to consider</param>
        /// <param name="wasDeleted">if true, then he/she was deleted, otherwise he/she was added</param>
        /// <param name="userNameOfChange">who made this change?</param>
        /// <returns></returns>
        Task ChangeContactsOfEvent(long id, string contactName, bool wasDeleted, User userNameOfChange);
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class ModificationEntryService : ModificationEntryRepository, IModificationEntryService
    {
        private List<ModificationEntry> listWithCreation;
        private List<ModificationEntry> listWithDeletion;
        private IContactPossibilitiesEntryRepository contactPossEntriesRepo;

        public ModificationEntryService(CrmContext context, IContactPossibilitiesEntryRepository contactPossEntriesRepo, IContactRepository contactRepo) : base(context, contactRepo)
        {
            this.contactPossEntriesRepo = contactPossEntriesRepo;
        }

        public async Task CreateNewContactEntryAsync(User userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.CREATED, MODEL_TYPE.CONTACT, DATA_TYPE.NONE);
        }

        public async Task UpdateContactAsync(User usernameOfModification, Contact oldContact, Contact newContact, bool deleteEntries)
        {
            await Task.Run(() => ComparerForModificationEntryCreation.CompareContacts(oldContact, newContact, usernameOfModification, deleteEntries, out listWithCreation, out listWithDeletion, contactPossEntriesRepo.GetTotalAmountOfEntities() + 1));            
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
            await CreateNewEntryAsync(null, id, MODIFICATION.DELETED, MODEL_TYPE.CONTACT, DATA_TYPE.NONE);
        }

        public async Task UpdateContactByHistoryElementAsync(User userNameOfChange, long id, string historyElementContent)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.ADDED, MODEL_TYPE.CONTACT, DATA_TYPE.HISTORY_ELEMENT, "", historyElementContent);
        }

        public async Task UpdateOrganizationAsync(User usernameOfModification, Organization oldOrga, Organization newOrga, bool deleteEntries)
        {
            await Task.Run(() => ComparerForModificationEntryCreation.CompareOrganizations(oldOrga, newOrga, usernameOfModification, deleteEntries, out listWithCreation, out listWithDeletion, contactPossEntriesRepo.GetTotalAmountOfEntities() + 1));
        }

        public async Task CreateNewOrganizationEntryAsync(User userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.CREATED, MODEL_TYPE.ORGANIZATION, DATA_TYPE.NONE);
        }

        public async Task UpdateOrganizationByHistoryElementAsync(User userNameOfChange, long id, string historyElementContent)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.ADDED, MODEL_TYPE.ORGANIZATION, DATA_TYPE.HISTORY_ELEMENT, "", historyElementContent);
        }

        public async Task UpdateOrganizationByDeletionAsync(long id)
        {
            await CreateNewEntryAsync(null, id, MODIFICATION.DELETED, MODEL_TYPE.ORGANIZATION, DATA_TYPE.NONE);
        }

        public async Task UpdateEventsAsync(User usernameOfModification, Event oldEvent, EventDto newEvent, List<Contact> contactsParticipated, List<Organization> orgasParticipated)
        {
            await Task.Run(() => ComparerForModificationEntryCreation.CompareEvents(oldEvent, newEvent, usernameOfModification, out listWithCreation, out listWithDeletion, contactsParticipated, orgasParticipated));
        }

        public async Task CreateNewEventEntryAsync(User userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.CREATED, MODEL_TYPE.EVENT, DATA_TYPE.NONE);
        }

        public async Task UpdateEventByDeletionAsync(long id)
        {
            await CreateNewEntryAsync(null, id, MODIFICATION.DELETED, MODEL_TYPE.EVENT, DATA_TYPE.NONE);
        }

        public async Task ChangeContactsOfEvent(long id, string contactName, bool wasDeleted, User userNameOfChange)
        {
            await ChangeContactsOfSource(id, contactName, wasDeleted, userNameOfChange, MODEL_TYPE.EVENT);
        }

        public async Task ChangeEmployeesOfOrganization(long id, string contactName, bool wasDeleted, User userNameOfChange)
        {
            await ChangeContactsOfSource(id, contactName, wasDeleted, userNameOfChange, MODEL_TYPE.ORGANIZATION);
        }

        /// <summary>
        /// create a new modification entry for the info that either a new contact was added or removed from a list
        /// </summary>
        /// <param name="id">the model id</param>
        /// <param name="contactName">the contact to consider</param>
        /// <param name="wasDeleted">if true, then the contact was removed, otherwise it was added</param>
        /// <param name="userNameOfChange">who modified this?</param>
        /// <param name="modelType">the model type to consider</param>
        /// <returns></returns>
        private async Task ChangeContactsOfSource(long id, string contactName, bool wasDeleted, User userNameOfChange, MODEL_TYPE modelType)
        {
            if (wasDeleted)
            {
                await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.DELETED, modelType, DATA_TYPE.CONTACTS, contactName, "");
            }
            else
            {
                await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.ADDED, modelType, DATA_TYPE.CONTACTS, "", contactName);
            }
        }
    }
}
