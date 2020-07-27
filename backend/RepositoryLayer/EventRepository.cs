using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    public interface IEventRepository : IBaseRepository<Event>
    {
        /// <summary>
        /// Getter für event mit allen includes anhand id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>event</returns>
        Task<Event> GetEventByIdWithAllIncludesAsync(long id);

        /// <summary>
        /// Getter für alle events als liste mit allen includes
        /// </summary>
        /// <returns>liste aller events</returns>
        Task<List<Event>> GetAllEventsWithAllIncludesAsync();

        /// <summary>
        /// Erzeuge ein neues event
        /// </summary>
        /// <param name="eventToCreate">das event welches erzeugt werden soll</param>
        /// <returns>das erzeugte event</returns>
        Task<Event> CreateNewEventAsync(EventCreateDto eventToCreate);

        /// <summary>
        /// Bearbeite einen existierenden event
        /// </summary>
        /// <param name="eventToModify">event zum bearbeiten</param>
        /// <returns>true wenn korrekt angepasst, ansonsten false</returns>
        Task<bool> ModifyEventAsync(EventDto eventToModify);

        /// <summary>
        /// Hinzufügen eines eventContacts
        /// </summary>
        /// <param name="eventContact">das eventContact</param>
        /// <returns>das erzeugte EventContact</returns>
        Task<EventContact> AddEventContactAsync(EventContact eventContact);

        /// <summary>
        /// Löschen eines eventContacts
        /// </summary>
        /// <param name="eventContact">das eventContact</param>
        /// <returns>true wenn alles ok, ansonsten false</returns>
        Task<bool> RemoveEventContactAsync(EventContact eventContact);
    }

    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        private IContactRepository contactRepo;
        private IEventContactRepository eventContactRepo;

        public EventRepository(CrmContext context, IEventContactRepository eventContactRepo, IContactRepository contactRepo) : base(context)
        {
            this.contactRepo = contactRepo;
            this.eventContactRepo = eventContactRepo;
        }

        public async Task<Event> CreateNewEventAsync(EventCreateDto eventToCreate)
        {
            Event eventNew = new Event();
            eventNew.Date = eventToCreate.Date;
            eventNew.Duration = eventToCreate.Duration;
            eventNew.Name = eventToCreate.Name;
            eventNew.Time = eventToCreate.Time;
            eventToCreate.Contacts.ForEach(x => eventNew.Participated.Add(new Participated() { ContactId = x, HasParticipated = false }));
            return await CreateAsync(eventNew);
        }

        public async Task<List<Event>> GetAllEventsWithAllIncludesAsync()
        {
            return await Entities.Include(y => y.Contacts).ThenInclude(z => z.Contact).Include(x => x.Participated).ToListAsync();
        }

        public async Task<Event> GetEventByIdWithAllIncludesAsync(long id)
        {
            return await Entities
                .Include(t => t.Tags)
                .Include(y => y.Contacts)
                .ThenInclude(z => z.Contact)
                .Include(x => x.Participated)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ModifyEventAsync(EventDto eventToModify)
        {
            Event eventExistent = await GetEventByIdWithAllIncludesAsync(eventToModify.Id);
            if (eventExistent != null)
            {
                List<EventContact> eventContacts = await eventContactRepo.GetAllAsync();
                eventContacts.RemoveAll(y => y.EventId != eventExistent.Id);
                eventExistent.Name = eventToModify.Name;
                eventExistent.Date = eventToModify.Date;
                eventExistent.Time = eventToModify.Time;
                eventExistent.Duration = eventToModify.Duration;
                List<EventContact> eventContactsToDelete = new List<EventContact>();
                eventContacts.ForEach(x =>
                { 
                    if (eventToModify.Contacts.FirstOrDefault(y => y.Id == x.ContactId) == null)
                    {
                        eventContactsToDelete.Add(x);
                    }
                });
                foreach (EventContact part in eventContactsToDelete)
                {
                    Participated participated = eventExistent.Participated.FirstOrDefault(x => x.ContactId == part.ContactId);
                    if (participated != null)
                    {
                        eventExistent.Participated.Remove(participated);
                    }
                    eventExistent.Contacts.Remove(part);
                    await RemoveEventContactAsync(part);
                }
                eventToModify.Participated.ForEach(x =>
                {
                    Participated participated = eventExistent.Participated.FirstOrDefault(y => y.Id == x.Id);
                    if (participated != null)
                    {
                        participated.HasParticipated = x.HasParticipated;
                    } else
                    {
                        eventExistent.Participated.Add(new Participated() { ContactId = x.ContactId, HasParticipated = x.HasParticipated });
                    }
                });
                
                foreach (ContactDto contact in eventToModify.Contacts) 
                {
                    if (eventContacts.FirstOrDefault(y => y.ContactId == contact.Id) == null)
                    {
                        await AddEventContactAsync(new EventContact() { ContactId = contact.Id, EventId = eventExistent.Id });
                    }
                }

                List<TagEvent> tagsToAdd = new List<TagEvent>();
                List<TagEvent> tagsToRemove = new List<TagEvent>();
                foreach (TagDto tag in eventToModify.Tags)
                {
                    if (eventExistent.Tags.Find(a => a.Name.Equals(tag.Name)) == null)
                    {
                        tagsToAdd.Add(new TagEvent() { Id=0, Name=tag.Name });
                    }
                }
                foreach (TagEvent tag in eventExistent.Tags)
                {
                    if (eventToModify.Tags.Find(a => a.Name.Equals(tag.Name)) == null)
                    {
                        tagsToRemove.Add(tag);
                    }
                }
                foreach (TagEvent tag in tagsToRemove)
                {
                    eventExistent.Tags.Remove(tag);
                }
                foreach (TagEvent tag in tagsToAdd)
                {
                    eventExistent.Tags.Add(tag);
                }

                await UpdateAsync(eventExistent);
                return true;
            }

            return false;
        }

        public async Task<EventContact> AddEventContactAsync(EventContact eventContact)
        {
            Contact contact = await contactRepo.GetByIdAsync(eventContact.ContactId);
            if (contact != null)
            {
                Event eventToInclude = await GetByIdAsync(eventContact.EventId);
                if (eventToInclude != null)
                {
                    EventContact result = await eventContactRepo.CreateAsync(new EventContact()
                    {
                        Contact = contact,
                        Event = eventToInclude,
                        ContactId = eventContact.ContactId,
                        EventId = eventContact.EventId
                    });
                    contact.Events.Add(result);
                    eventToInclude.Contacts.Add(result);
                    await contactRepo.UpdateAsync(contact);
                    await UpdateAsync(eventToInclude);
                    return result;
                }
            }

            return null;
        }

        public async Task<bool> RemoveEventContactAsync(EventContact eventContact)
        {
            EventContact eventContactToDelete = await eventContactRepo.GetEventContactByEventContactAsync(eventContact);
            if (eventContactToDelete != null)
            {
                await eventContactRepo.DeleteAsync(eventContactToDelete);
                return true;
            }

            return false;
        }
    }
}
