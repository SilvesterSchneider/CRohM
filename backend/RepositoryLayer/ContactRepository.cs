using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer
{
    public interface IContactRepository : IBaseRepository<Contact>
    {
        /// <summary>
        /// Get the contacts which name or preName starts with the given string
        /// </summary>
        /// <param name="name">the name to be searched for</param>
        /// <returns>a list containing all Contacts</returns>
        Task<List<Contact>> GetContactsByPartStringAsync(string name);

        /// <summary>
        /// Returns a full list of all contacts and its all dependencies.
        /// </summary>
        /// <returns></returns>
        Task<List<Contact>> GetAllContactsWithAllIncludesAsync();

        Task<bool> UpdateAsync(Contact contact, long id);
    }

    public class ContactRepository : BaseRepository<Contact>, IContactRepository
    {
        public ContactRepository(CrmContext context) : base(context) { }

        public async Task<List<Contact>> GetAllContactsWithAllIncludesAsync()
        {
            return await Entities
                .Include(x => x.Address)
                .Include(y => y.ContactPossibilities)
                .ThenInclude(b => b.ContactEntries)
                .Include(x => x.OrganizationContacts)
                .ThenInclude(a => a.Organization)
                .Include(c => c.Events)
                .ThenInclude(d => d.Event)
                .ThenInclude(e => e.Participated)
                .ToListAsync();
        }

        public override async Task<Contact> GetByIdAsync(long id)
        {
            return await Entities.Include(a => a.Address).Include(b => b.ContactPossibilities).ThenInclude(b => b.ContactEntries).Include(c => c.Events).ThenInclude(d => d.Event).FirstAsync(x => x.Id == id);
        }

        public async Task<List<Contact>> GetContactsByPartStringAsync(string name)
        {
            return await Entities
                .Where(x => x.PreName.StartsWith(name) | x.Name.StartsWith(name))
                .Include(x => x.Address)
                .Include(y => y.ContactPossibilities)
                .ThenInclude(b => b.ContactEntries)
                .Include(c => c.Events)
                .ThenInclude(d => d.Event)
                .ThenInclude(e => e.Participated)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Contact contact, long id)
        {
            Contact originalContact = await Entities.Include(x => x.Address).Include(y => y.ContactPossibilities).ThenInclude(b => b.ContactEntries).FirstAsync(x => x.Id == id);
            if (originalContact != null)
            {
                originalContact.Address.Street = contact.Address.Street;
                originalContact.Address.StreetNumber = contact.Address.StreetNumber;
                originalContact.Address.City = contact.Address.City;
                originalContact.Address.Country = contact.Address.Country;
                originalContact.Address.Zipcode = contact.Address.Zipcode;
                originalContact.ContactPossibilities.Fax = contact.ContactPossibilities.Fax;
                originalContact.ContactPossibilities.Mail = contact.ContactPossibilities.Mail;
                originalContact.ContactPossibilities.PhoneNumber = contact.ContactPossibilities.PhoneNumber;
                List<ContactPossibilitiesEntry> toBeDeleted = new List<ContactPossibilitiesEntry>();
                foreach (ContactPossibilitiesEntry entry in originalContact.ContactPossibilities.ContactEntries)
                {
                    if (contact.ContactPossibilities.ContactEntries.FirstOrDefault(x => x.Id == entry.Id) == null)
                    {
                        toBeDeleted.Add(entry);
                    }
                }

                foreach (ContactPossibilitiesEntry entry in toBeDeleted)
                {
                    originalContact.ContactPossibilities.ContactEntries.Remove(entry);
                }
                
                foreach (ContactPossibilitiesEntry entry in contact.ContactPossibilities.ContactEntries)
                {
                    if (entry.Id != 0)
                    {
                        ContactPossibilitiesEntry existentEntry = originalContact.ContactPossibilities.ContactEntries.FirstOrDefault(x => x.Id == entry.Id);
                        if (existentEntry != null)
                        {
                            existentEntry.ContactEntryValue = entry.ContactEntryValue;
                            existentEntry.ContactEntryName = entry.ContactEntryName;
                        }
                    }
                    else
                    {
                        originalContact.ContactPossibilities.ContactEntries.Add(entry);
                    }                    
                }
                originalContact.Description = contact.Description;
                originalContact.Name = contact.Name;
                originalContact.PreName = contact.PreName;
                await UpdateAsync(originalContact);
                return true;
            } 
            else
            {
                return false;
            }
        }
    }
}
