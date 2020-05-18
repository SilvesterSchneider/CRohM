using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;

namespace RepositoryLayer
{
    public interface IOrganizationContactRepository : IBaseRepository<OrganizationContact>
    {
        /// <summary>
        /// Check if entity exists async
        /// </summary>
        /// <param name="contactId">id of contact</param>
        /// <param name="organizationId">id of organization</param>
        /// <returns></returns>
        Task<bool> CheckIfOrganizationContactExistsAsync(long contactId, long organizationId);

        Task<OrganizationContact> GetOrganizationContactByIdsAsync(long contactId, long organizationId);
    }

    public class OrganizationContactRepository : BaseRepository<OrganizationContact>, IOrganizationContactRepository
    {
        public OrganizationContactRepository(CrmContext context) : base(context)
        {
        }

        public async Task<bool> CheckIfOrganizationContactExistsAsync(long contactId, long organizationId)
        {
            return await Entities.AnyAsync(x => x.ContactId == contactId && x.OrganizationId == organizationId);
        }

        public async Task<OrganizationContact> GetOrganizationContactByIdsAsync(long contactId, long organizationId)
        {
            return await Entities.FirstOrDefaultAsync(x => x.ContactId == contactId && x.OrganizationId == organizationId);
        }
    }
}