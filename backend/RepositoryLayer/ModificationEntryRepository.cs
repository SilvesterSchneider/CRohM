using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Helper;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
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
        Task<IdentityResult> UpdateModificationAsync(string userName, long id, MODEL_TYPE dataType);

        /// <summary>
        /// Create a new entry by dataModel id and datatype
        /// </summary>
        /// <param name="dataModelId">the id of the model which was changed</param>
        /// <param name="dataType">the data type of the changed model</param>
        /// <returns></returns>
        Task<IdentityResult> CreateNewEntryAsync(string userName, long dataModelId, MODEL_TYPE dataType);

        /// <summary>
        /// Remove an entry from DB
        /// </summary>
        /// <param name="id">the id of dataset</param>
        /// <param name="dataType">the datatype to be considered</param>
        /// <returns></returns>
        Task<IdentityResult> RemoveEntryAsync(long id, MODEL_TYPE dataType);

        /// <summary>
        /// Get a sorted list of all history entries beginning with the newest one.
        /// </summary>
        /// <param name="dataType">the data type to get it for</param>
        /// <returns>the sorted list</returns>
        Task<List<ModificationEntry>> GetSortedModificationEntriesByTypeAsync(MODEL_TYPE dataType);
    }

    public class ModificationEntryRepository : BaseRepository<ModificationEntry>, IModificationEntryRepository
    {
        public ModificationEntryRepository(CrmContext context) : base(context)
        {

        }

        public async Task<IdentityResult> CreateNewEntryAsync(string userName, long id, MODEL_TYPE dataType)
        {
            ModificationEntry entry = new ModificationEntry()
            {
                DataModelId = id,
                DateTime = DateTime.Now,
                DataModelType = dataType,
                ModificationType = MODIFICATION.CREATED,
                UserName = userName
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

        public async Task<IdentityResult> UpdateModificationAsync(string userName, long id, MODEL_TYPE dataType)
        {
            ModificationEntry entry = await GetModificationEntryByIdAndTypeAsync(id, dataType);
            if (entry != null)
            {
                entry.ModificationType = MODIFICATION.MODIFIED;
                entry.DateTime = DateTime.Now;
                entry.UserName = userName;

                if (await UpdateAsync(entry) != null)
                {
                    return IdentityResult.Success;
                }
            }
            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> RemoveEntryAsync(long id, MODEL_TYPE dataType)
        {
            ModificationEntry entry = await GetModificationEntryByIdAndTypeAsync(id, dataType);
            if (entry != null)
            {
                await DeleteAsync(entry);
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<List<ModificationEntry>> GetSortedModificationEntriesByTypeAsync(MODEL_TYPE dataType)
        {
            List<ModificationEntry> list = await Entities.Where(x => x.DataModelType == dataType).ToListAsync();
            list.Sort((x, y) => { return DateTime.Compare(y.DateTime, x.DateTime); });
            return list;
        }

        private async Task<ModificationEntry> GetModificationEntryByIdAndTypeAsync(long id, MODEL_TYPE dataType)
        {
            ModificationEntry entry = await Entities.FirstOrDefaultAsync(x => x.DataModelId == id && x.DataModelType == dataType);
            return entry;
        }
    }
}
