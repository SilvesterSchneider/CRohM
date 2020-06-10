using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    public interface IEventContactRepository : IBaseRepository<EventContact>
    {
        /// <summary>
        /// Getter für eventContact anhand eines anderen event kontakts
        /// </summary>
        /// <param name="eventContact">der event kontakt dessen ids abgefragt werden sollen</param>
        /// <returns>eventContact</returns>
        Task<EventContact> GetEventContactByEventContactAsync(EventContact eventContact);

        /// <summary>
        /// Getter für alle einträge als liste
        /// </summary>
        /// <returns>Liste mit allen eventContacts</returns>
        Task<List<EventContact>> GetAllAsync();
    }

    public class EventContactRepository : BaseRepository<EventContact>, IEventContactRepository
    {
        public EventContactRepository(CrmContext context) : base(context) 
        {
        }

        public async Task<List<EventContact>> GetAllAsync()
        {
            return await Entities.Include(x => x.Contact).Include(y => y.Event).ToListAsync();
        }

        public async Task<EventContact> GetEventContactByEventContactAsync(EventContact eventContact)
        {
            return await Entities.Include(x => x.Contact).Include(y => y.Event).FirstOrDefaultAsync(x => x.ContactId == eventContact.ContactId && x.EventId == eventContact.EventId);
        }
    }
}
