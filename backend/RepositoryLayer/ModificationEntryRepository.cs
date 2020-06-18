﻿using Microsoft.AspNetCore.Identity;
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
        Task<IdentityResult> UpdateModificationAsync(long id, MODEL_TYPE dataType);
        Task<IdentityResult> CreateNewEntryAsync(long dataModelId, MODEL_TYPE dataType);
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

        public async Task<IdentityResult> CreateNewEntryAsync(long id, MODEL_TYPE dataType)
        {
            ModificationEntry entry = new ModificationEntry()
            {
                DataModelId = id,
                DateTime = DateTime.Now,
                DataModelType = dataType,
                ModificationType = MODIFICATION.CREATED,
                UserName = GetLoggedInUserName()
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

        public async Task<IdentityResult> UpdateModificationAsync(long id, MODEL_TYPE dataType)
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
