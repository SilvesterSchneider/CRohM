using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    public interface IModificationEntryRepository : IBaseRepository<ModificationEntry>
    {
        /// <summary>
        /// set entries to deletion state after a datenschutzbeauftragter cleared the field
        /// </summary>
        /// <param name="id">the id of dataset to be updated</param>
        /// <param name="dataType">the datatype to be updated</param>
        /// <returns></returns>
        Task<IdentityResult> SetEntriesToDeletionStateAsync(long id, MODEL_TYPE modelType, DATA_TYPE dataType, int extensionIndex = -1);

        /// <summary>
        /// Create a new entry by dataModel id and datatype
        /// </summary>
        /// <param name="dataModelId">the id of the model which was changed</param>
        /// <param name="dataType">the data type of the changed model</param>
        /// <returns></returns>
        Task<IdentityResult> CreateNewEntryAsync(
            User user,
            long dataModelId,
            MODIFICATION modificationType,
            MODEL_TYPE modelType,
            DATA_TYPE dataType,
            string oldValue = "",
            string actualValue = "",
            int extensionIndex = -1
            );

        /// <summary>
        /// Get a sorted list of all history entries beginning with the newest one.
        /// </summary>
        /// <param name="dataType">the data type to get it for</param>
        /// <returns>the sorted list</returns>
        Task<List<ModificationEntry>> GetSortedModificationEntriesByModelDataTypeAsync(MODEL_TYPE dataType, User user);

        /// <summary>
        /// get all entries for a specific id and datatype.
        /// </summary>
        /// <param name="id">the model id</param>
        /// <param name="dataType">the datatype</param>
        /// <returns></returns>
        Task<List<ModificationEntry>> GetModificationEntriesByIdAndModelTypeAsync(long id, MODEL_TYPE dataType);

        /// <summary>
        /// Get all modification entries just for creation of specific model type.
        /// </summary>
        /// <param name="modelType">the modeltype to consider</param>
        /// <returns></returns>
        Task<List<ModificationEntry>> GetModificationEntriesForCreationByModelType(MODEL_TYPE modelType);


        /// <summary>
        /// get all entries for a specific id and datatype with pagination
        /// </summary>
        /// <param name="id">the model id</param>
        /// <param name="dataType">the datatype</param>
        /// <param name="pageStart">starting position for pagination</param>
        /// <param name="pageSize">Number of elements requested by pagination</param>
        /// <returns></returns>
        Task<List<ModificationEntry>> GetModificationEntriesByIdAndModelTypePaginationAsync(long id, MODEL_TYPE dataType, int pageStart, int pageSize);

        /// <summary>
        /// get count of all entries for a specific id and datatype 
        /// </summary>
        /// <param name="id">the model id</param>
        /// <param name="dataType">the datatype</param>
        /// <returns></returns>
        Task<int> GetModificationEntriesByIdAndModelTypeCountAsync(long id, MODEL_TYPE dataType);


        /// <summary>
        /// To be able to delete a user, the foreign key relation in ModificationEntry needs to be set to null 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task RemoveUserForeignKeys(User user);
    }

    public class ModificationEntryRepository : BaseRepository<ModificationEntry>, IModificationEntryRepository
    {
        private IContactRepository contactRepository;

        public ModificationEntryRepository(CrmContext context, IContactRepository contactRepository) : base(context)
        {
            this.contactRepository = contactRepository;
        }

        public async Task<IdentityResult> CreateNewEntryAsync(
            User user,
            long dataModelId,
            MODIFICATION modificationType,
            MODEL_TYPE modelType,
            DATA_TYPE dataType,
            string oldValue = "",
            string actualValue = "",
            int extensionIndex = -1)
        {
            ModificationEntry entry = new ModificationEntry()
            {
                DataModelId = dataModelId,
                DateTime = DateTime.Now,
                DataModelType = modelType,
                ModificationType = modificationType,
                User = user,
                ActualValue = actualValue,
                DataType = dataType,
                ExtensionIndex = extensionIndex,
                OldValue = oldValue
            };
            if (await CreateAsync(entry) != null)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> SetEntriesToDeletionStateAsync(long id, MODEL_TYPE modelType, DATA_TYPE dataType, int extensionIndex = -1)
        {
            bool success = true;
            List<ModificationEntry> entries = await GetModificationEntriesByIdAndModelTypeAsync(id, modelType);
            foreach (ModificationEntry entry in entries)
            {
                if (entry.DataType == dataType && entry.ExtensionIndex == extensionIndex)
                {
                    entry.SetToDeletionState();
                    if (await UpdateAsync(entry) == null)
                    {
                        success = false;
                    }
                }
            }
            if (success)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }

        public async Task<List<ModificationEntry>> GetSortedModificationEntriesByModelDataTypeAsync(MODEL_TYPE dataType, User user)
        {
            List<ModificationEntry> list = await Entities.Include(u => u.User).Where(x => x.DataModelType == dataType).ToListAsync();
            List<ModificationEntry> listToDelete = new List<ModificationEntry>();
            foreach (ModificationEntry entry in list)
            {
                if (entry.ModificationType == MODIFICATION.DELETED && entry.DataType == DATA_TYPE.NONE)
                {
                    listToDelete.AddRange(list.FindAll(a => a.DataModelId == entry.DataModelId));
                }
            }
            if (dataType == MODEL_TYPE.CONTACT)
            {
                List<Contact> allContacts = await contactRepository.GetAllUnapprovedContactsWithAllIncludesAsync();
                if (allContacts != null && allContacts.Any())
                {
                    foreach (ModificationEntry entry in list)
                    {
                        if (allContacts.Any(b => !user.IsSuperAdmin && b.Id == entry.DataModelId && b.CreatedByUser != user.Id))
                        {
                            listToDelete.Add(entry);
                        }
                    }
                }
            }
            listToDelete.ForEach(y => list.Remove(y));
            list.Sort((x, y) => { return DateTime.Compare(y.DateTime, x.DateTime); });
            return list;
        }

        public async Task<List<ModificationEntry>> GetModificationEntriesByIdAndModelTypeAsync(long id, MODEL_TYPE dataType)
        {
            return await Entities
                .Include(x => x.User)
                .Where(x => x.DataModelId == id && x.DataModelType == dataType).ToListAsync();
        }


        public async Task<List<ModificationEntry>> GetModificationEntriesByIdAndModelTypePaginationAsync(long id, MODEL_TYPE dataType, int pageStart, int pageSize)
        {
            return await Entities
                .Include(x => x.User)
                .Where(x => x.DataModelId == id && x.DataModelType == dataType)
                .OrderByDescending(x => x.DateTime)
                .Skip(pageStart)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetModificationEntriesByIdAndModelTypeCountAsync(long id, MODEL_TYPE dataType)
        {
            return await Entities
                .Include(x => x.User)
                .Where(x => x.DataModelId == id && x.DataModelType == dataType)
                .CountAsync();
        }

        public async Task RemoveUserForeignKeys(User user)
        {
            List<ModificationEntry> entities = await Entities.Where(entry => entry.User == user).ToListAsync();
            entities.ForEach(entry => entry.User = null);

            if (entities.Count > 0)
            {
                await this.UpdateRangeAsync(entities);
            }
        }

        public async Task<List<ModificationEntry>> GetModificationEntriesForCreationByModelType(MODEL_TYPE modelType)
        {
            return await Entities.Where(entry => entry.DataModelType == modelType && entry.DataType == DATA_TYPE.NONE).ToListAsync();
        }
    }
}
