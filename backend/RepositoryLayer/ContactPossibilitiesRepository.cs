using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    public interface IContactPossibilitiesRepository : IBaseRepository<ContactPossibilities>
    {
        Task<ContactPossibilities> GetContactPossibilityByIdAsync(long contactPossibilitiesId);
        Task<bool> AddContactPossibilityEntryAsync(ContactPossibilitiesEntry entry, long contactPossibilitiesId);

        Task RemoveContactPossibilityEntryAsync(long contactPossibilityId, long contactPossibilityEntryId);
    }

    public class ContactPossibilitiesRepository : BaseRepository<ContactPossibilities>, IContactPossibilitiesRepository
    {
        public ContactPossibilitiesRepository(CrmContext context) : base(context) { }

        public async Task<bool> AddContactPossibilityEntryAsync(ContactPossibilitiesEntry entry, long contactPossibilitiesId)
        {
            ContactPossibilities contact = await GetContactPossibilityByIdAsync(contactPossibilitiesId);
            if (contact != null)
            {
                bool found = false;
                foreach (ContactPossibilitiesEntry entryExistent in contact.ContactEntries)
                {
                    if (entryExistent.Id == entry.Id)
                    {
                        found = true;
                        entryExistent.ContactEntryName = entry.ContactEntryName;
                        entryExistent.ContactEntryValue = entry.ContactEntryValue;
                    }
                }

                if (!found)
                {
                    contact.ContactEntries.Add(entry);
                }

                await UpdateAsync(contact);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ContactPossibilities> GetContactPossibilityByIdAsync(long id)
        {
            return await Entities.Include(y => y.ContactEntries).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task RemoveContactPossibilityEntryAsync(long contactPossibilityId, long contactPossibilityEntryId)
        {
            ContactPossibilities contact = await GetContactPossibilityByIdAsync(contactPossibilityId);
            if (contact != null)
            {
                ContactPossibilitiesEntry entry = null;
                foreach (ContactPossibilitiesEntry entryExistend in contact.ContactEntries)
                {
                    if (entryExistend.Id == contactPossibilityEntryId)
                    {
                        entry = entryExistend;
                        break;
                    }
                }
                if (entry != null)
                {
                    contact.ContactEntries.Remove(entry);
                    await UpdateAsync(contact);
                }
            }
        }
    }
}
