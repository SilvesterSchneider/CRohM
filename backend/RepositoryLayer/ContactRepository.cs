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
        private IAddressRepository addressRepository;
        private IContactPossibilitiesRepository contactPossibilitiesRepository;

        public ContactRepository(CrmContext context, IAddressRepository addressRepository, IContactPossibilitiesRepository contactPossibilitiesRepository) : base(context) {
            this.addressRepository = addressRepository;
            this.contactPossibilitiesRepository = contactPossibilitiesRepository;
        }

        public async Task<List<Contact>> GetAllContactsWithAllIncludesAsync()
        {
            return await Entities
                .Include(x => x.Address)
                .Include(y => y.ContactPossibilities)
                .ThenInclude(b => b.ContactEntries)
                .Include(x => x.OrganizationContacts)
                .ThenInclude(a => a.Organization)
                .ToListAsync();
        }

        public override async Task<Contact> GetByIdAsync(long id)
        {
            return await Entities.Include(a => a.Address).Include(b => b.ContactPossibilities).ThenInclude(b => b.ContactEntries).FirstAsync(x => x.Id == id);
        }

        public async Task<List<Contact>> GetContactsByPartStringAsync(string name)
        {
            return await Entities
                .Where(x => x.PreName.StartsWith(name) | x.Name.StartsWith(name))
                .Include(x => x.Address)
                .Include(y => y.ContactPossibilities)
                .ThenInclude(b => b.ContactEntries)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Contact contact, long id)
        {
            Contact originalContact = await Entities.Include(x => x.Address).Include(y => y.ContactPossibilities).ThenInclude(b => b.ContactEntries).FirstAsync(x => x.Id == id);
            if (originalContact != null)
            {                
                originalContact.Address = await getAddress(contact.Address);
                originalContact.ContactPossibilities = await getContactPossibilities(contact.ContactPossibilities);
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

        private async Task<ContactPossibilities> getContactPossibilities(ContactPossibilities contactPossibilities)
        {
            ContactPossibilities possibilities = await contactPossibilitiesRepository.GetContactPossibilityByIdAsync(contactPossibilities.Id);
            possibilities.Fax = contactPossibilities.Fax;
            possibilities.Mail = contactPossibilities.Mail;
            possibilities.PhoneNumber = contactPossibilities.PhoneNumber;
            return possibilities;
        }

        private async Task<Address> getAddress(Address addressUpdated)
        {
            Address address = await addressRepository.GetByIdAsync(addressUpdated.Id);
            address.Street = addressUpdated.Street;
            address.StreetNumber = addressUpdated.StreetNumber;
            address.City = addressUpdated.City;
            address.Country = addressUpdated.Country;
            address.Zipcode = addressUpdated.Zipcode;
            return address;
        }
    }
}
