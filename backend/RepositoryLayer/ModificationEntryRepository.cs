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
        Task UpdateModificationAsync(long id, MODEL_TYPE dataType);
        Task CreateNewEntryAsync(long id, MODEL_TYPE dataType);
        Task RemoveEntryAsync(long id, MODEL_TYPE dataType);
        Task<List<ModificationEntry>> GetSortedModificationEntriesByTypeAsync(MODEL_TYPE dataType);
    }

    public class ModificationEntryRepository : BaseRepository<ModificationEntry>, IModificationEntryRepository
    {
        public ModificationEntryRepository(CrmContext context) : base(context)
        {

        }

        public async Task CreateNewEntryAsync(long id, MODEL_TYPE dataType)
        {
            ModificationEntry entry = new ModificationEntry()
            {
                DataModelId = id,
                DateTime = DateTime.Now,
                DataModelType = dataType,
                ModificationType = MODIFICATION.CREATED,
                UserName = GetLoggedInUserName()
            };
            await CreateAsync(entry);
        }

        public async Task UpdateModificationAsync(long id, MODEL_TYPE dataType)
        {
            ModificationEntry entry = await GetModificationEntryByIdAndTypeAsync(id, dataType);
            if (entry != null)
            {
                entry.ModificationType = MODIFICATION.MODIFIED;
                entry.DateTime = DateTime.Now;
                string userName = GetLoggedInUserName();
                if (!string.IsNullOrEmpty(userName))
                {
                    entry.UserName = userName;
                }
                
                await UpdateAsync(entry);
            }
        }

        public async Task RemoveEntryAsync(long id, MODEL_TYPE dataType)
        {
            ModificationEntry entry = await GetModificationEntryByIdAndTypeAsync(id, dataType);
            if (entry != null)
            {
                await DeleteAsync(entry);
            }
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

        private string GetLoggedInUserName()
        {
            string userName = string.Empty;
            if (LoggedInUser.GetLoggedInUser() != null)
            {
                userName = LoggedInUser.GetLoggedInUser().FirstName + " " + LoggedInUser.GetLoggedInUser().LastName;
                if (string.IsNullOrEmpty(userName.Trim()))
                {
                    userName = "admin";
                }
            }
            return userName;
        }
    }
}
