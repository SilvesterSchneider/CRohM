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
        /// Update the entry by new data
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
            string userName,
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

        Task<List<ModificationEntry>> GetModificationEntriesByIdAndModelTypeAsync(long id, MODEL_TYPE dataType);
    }

    public class ModificationEntryRepository : BaseRepository<ModificationEntry>, IModificationEntryRepository
    {
        public ModificationEntryRepository(CrmContext context) : base(context)
        {

        }

        public async Task<IdentityResult> CreateNewEntryAsync(
            string userName,
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
                UserName = userName,
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
                    entry.IsDeleted = true;
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
            List<ModificationEntry> list = await Entities.Where(x => x.DataModelType == dataType).ToListAsync();
            list.Sort((x, y) => { return DateTime.Compare(y.DateTime, x.DateTime); });
            return list;
        }

        public async Task<List<ModificationEntry>> GetModificationEntriesByIdAndModelTypeAsync(long id, MODEL_TYPE dataType)
        {
            return await Entities.Where(x => x.DataModelId == id && x.DataModelType == dataType).ToListAsync();
        }
    }
}
