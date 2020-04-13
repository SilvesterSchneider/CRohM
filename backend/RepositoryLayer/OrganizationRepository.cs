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
        /// <summary>
        /// Get the organizations just in dependency on the contact who should be an employee in that company.
        /// </summary>
        /// <param name="contact">the contact to be searched for</param>
        /// <returns>a list containing all organizations</returns>
        Task<List<Organization>> GetOrganizationsByContactAsync(Contact contact);
        Task<List<Organization>> GetAllOrganizationsWithIncludesAsync();
        Task<List<Contact>> GetAllContactsOfAnOrganizationAsync(long id);
    }

    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(CrmContext context) : base(context) { }

        public async Task<List<Contact>> GetAllContactsOfAnOrganizationAsync(long id)
        {
            Organization org = await Entities.FirstAsync(x => x.Id == id);
            if (org != null && org.Employees != null)
            {
                return org.Employees;
            } else
            {
                return new List<Contact>();
            }
        }

        public Task<List<Organization>> GetAllOrganizationsWithIncludesAsync()
        {
            return Entities.Include(x => x.Address).Include(y => y.Contact).ToListAsync();
        }

        public async Task<List<Organization>> GetOrganizationsByContactAsync(Contact contact)
        {
            return await Entities
                .Where(x => x.Employees.Contains(contact))
                .Include(x => x.Address)
                .Include(y => y.Contact)
                .ToListAsync();
        }
    }
}
