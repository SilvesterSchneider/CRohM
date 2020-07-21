using ModelLayer;
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
        Task CreateNewContactEntryAsync(string userNameOfChange, long id);
        Task UpdateContactByHistoryElementAsync(string userNameOfChange, long id);
        Task UpdateContactByDeletionAsync(long id);
    }

    public class ModificationEntryService : ModificationEntryRepository, IModificationEntryService
    {
        public ModificationEntryService(CrmContext context) : base(context)
        {

        }

        public async Task CreateNewContactEntryAsync(string userNameOfChange, long id)
        {
            await CreateNewEntryAsync(userNameOfChange, id, MODIFICATION.CREATED, MODEL_TYPE.CONTACT, DATA_TYPE.NONE);
        }

        public async Task UpdateContactAsync(string usernameOfModification, Contact oldContact, Contact newContact, bool deleteEntries)
        {
            List<ModificationEntry> listWithCreation;
            List<ModificationEntry> listWithDeletion;
            ComparerForModificationEntryCreation.CompareContacts(oldContact, newContact, usernameOfModification, deleteEntries, out listWithCreation, out listWithDeletion),
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
    }
}
