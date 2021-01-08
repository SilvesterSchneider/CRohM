using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public interface IEventOrganizationRepository :IBaseRepository<EventOrganization>
    {
        /// <summary>
        /// Getter für eventContact anhand eines anderen event kontakts
        /// </summary>
        /// <param name="eventContact">der event kontakt dessen ids abgefragt werden sollen</param>
        /// <returns>eventContact</returns>
        Task<EventOrganization> GetEventOrganizationByEventOrganizationAsync(EventOrganization eventContact);

        /// <summary>
        /// Getter für alle einträge als liste
        /// </summary>
        /// <returns>Liste mit allen eventContacts</returns>
        Task<List<EventOrganization>> GetAllAsync();
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class EventOrganizationRepository : BaseRepository<EventOrganization>, IEventOrganizationRepository
    {
        public EventOrganizationRepository(CrmContext context) : base(context)
        {

        }

        public async Task<List<EventOrganization>> GetAllAsync()
        {
            return await Entities.Include(x => x.Organization).Include(y => y.Event).ToListAsync();
        }

        public async Task<EventOrganization> GetEventOrganizationByEventOrganizationAsync(EventOrganization eventContact)
        {
            return await Entities.Include(x => x.Organization).Include(y => y.Event).FirstOrDefaultAsync(x => x.OrganizationId == eventContact.OrganizationId && x.EventId == eventContact.EventId);
        }
    }
}
