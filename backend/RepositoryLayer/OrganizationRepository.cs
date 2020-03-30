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
        /// Get the organizations just in dependency on the user who should be an employee in that company.
        /// </summary>
        /// <param name="user">the user to be searched for</param>
        /// <returns>a list containing all organizations</returns>
        Task<List<Organization>> GetOrganizationsByUserAsync(User user);
    }

    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(CrmContext context) : base(context) { }

        public async Task<List<Organization>> GetOrganizationsByUserAsync(User user)
        {
            return await Entities
                .Where(x => x.Employees.Contains(user))
                .ToListAsync();
        }
    }
}
