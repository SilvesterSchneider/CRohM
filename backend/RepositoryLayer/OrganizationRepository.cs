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
    }
}