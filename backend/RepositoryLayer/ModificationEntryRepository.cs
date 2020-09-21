using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        Task<List<ModificationEntry>> GetSortedModificationEntriesByModelDataTypeAsync(MODEL_TYPE dataType);

        /// <summary>
        /// get all entries for a specific id and datatype.
        /// </summary>
        /// <param name="id">the model id</param>
        /// <param name="dataType">the datatype</param>
        /// <returns></returns>
        Task<List<ModificationEntry>> GetModificationEntriesByIdAndModelTypeAsync(long id, MODEL_TYPE dataType);


        /// <summary>
        /// To be able to delete a user, the foreign key relation in ModificationEntry needs to be set to null 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<List<ModificationEntry>> RemoveUserForeignKeys(User user);
    }

    public class ModificationEntryRepository : BaseRepository<ModificationEntry>, IModificationEntryRepository
    {
        public ModificationEntryRepository(CrmContext context) : base(context)
        {

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

        public async Task<List<ModificationEntry>> GetSortedModificationEntriesByModelDataTypeAsync(MODEL_TYPE dataType)
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

        public async Task<List<ModificationEntry>> RemoveUserForeignKeys(User user)
        {
            List<ModificationEntry> entities = await Entities.Where(entry => entry.User == user).ToListAsync();
            entities.ForEach(entry => entry.User = null);

            if (entities.Count > 0)
            {
                return await this.UpdateRangeAsync(entities);

            }

            return new List<ModificationEntry>();
        }

    }
}
