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
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
        Task<List<Organization>> GetAllOrganizationsWithIncludesAsync();

        Task<List<Contact>> GetAllContactsOfAnOrganizationAsync(long id);

        Task<bool> UpdateAsyncWithAlleDependencies(Organization newOrganization);
    }

    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(CrmContext context) : base(context)
        {
        }

        public async Task<List<Contact>> GetAllContactsOfAnOrganizationAsync(long id)
        {
            Organization org = await Entities
                .Include(a => a.Contact)
                .ThenInclude(b => b.ContactEntries)
                .Include(x => x.OrganizationContacts)
                .ThenInclude(x => x.Contact)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (org != null && org.OrganizationContacts.Any())
            {
                return org.OrganizationContacts
                    .Select(contact => contact.Contact)
                    .ToList();
            }
            return new List<Contact>();
        }

        public override async Task<Organization> GetByIdAsync(long id)
        {
            return await Entities
                .Include(x => x.Address)
                .Include(y => y.Contact)
                .ThenInclude(b => b.ContactEntries)
                .Include(z => z.OrganizationContacts)
                .ThenInclude(a => a.Contact)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task<List<Organization>> GetAllOrganizationsWithIncludesAsync()
        {
            return Entities
                .Include(x => x.Address)
                .Include(y => y.Contact)
                .ThenInclude(b => b.ContactEntries)
                .Include(z => z.OrganizationContacts)
                .ThenInclude(a => a.Contact).ToListAsync();
        }

        public async Task<bool> UpdateAsyncWithAlleDependencies(Organization newOrganization)
        {
            Organization organization = await Entities
                .Include(x => x.Address)
                .Include(y => y.OrganizationContacts)
                .Include(z => z.Contact)
                .ThenInclude(a => a.ContactEntries)
                .FirstOrDefaultAsync(b => b.Id == newOrganization.Id);
            if (organization != null)
            {
                organization.Name = newOrganization.Name;
                organization.Description = newOrganization.Description;
                organization.Address.City = newOrganization.Address.City;
                organization.Address.Country = newOrganization.Address.Country;
                organization.Address.Street = newOrganization.Address.Street;
                organization.Address.StreetNumber = newOrganization.Address.StreetNumber;
                organization.Address.Zipcode = newOrganization.Address.Zipcode;
                organization.Contact.Fax = newOrganization.Contact.Fax;
                organization.Contact.PhoneNumber = newOrganization.Contact.PhoneNumber;
                organization.Contact.Mail = newOrganization.Contact.Mail;
                List<ContactPossibilitiesEntry> toBeDeleted = new List<ContactPossibilitiesEntry>();
                foreach (ContactPossibilitiesEntry entry in organization.Contact.ContactEntries)
                {
                    if (newOrganization.Contact.ContactEntries.FirstOrDefault(x => x.Id == entry.Id) == null)
                    {
                        toBeDeleted.Add(entry);
                    }
                }

                foreach (ContactPossibilitiesEntry entry in toBeDeleted)
                {
                    organization.Contact.ContactEntries.Remove(entry);
                }

                foreach (ContactPossibilitiesEntry entry in newOrganization.Contact.ContactEntries)
                {
                    if (entry.Id != 0)
                    {
                        ContactPossibilitiesEntry existentEntry = organization.Contact.ContactEntries.FirstOrDefault(x => x.Id == entry.Id);
                        if (existentEntry != null)
                        {
                            existentEntry.ContactEntryValue = entry.ContactEntryValue;
                            existentEntry.ContactEntryName = entry.ContactEntryName;
                        }
                    }
                    else
                    {
                        organization.Contact.ContactEntries.Add(entry);
                    }
                }
                await UpdateAsync(organization);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}