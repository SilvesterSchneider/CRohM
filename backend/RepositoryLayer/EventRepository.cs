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
        /// Get all requests for a contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        Task<List<Event>> GetEventsForContact(long contactId);

        /// <summary>
        /// Get all events for a specific organization.
        /// </summary>
        /// <param name="orgaId"></param>
        /// <returns></returns>
        Task<List<Event>> GetEventsForOrganization(long orgaId);

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

        Task<EventContact> AddEventContactAsync(EventContact eventContact);

        Task<EventOrganization> AddEventOrganizationAsync(EventOrganization eventOrganization);
    }

    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        private IContactRepository contactRepo;
        private IEventContactRepository eventContactRepo;
        private IOrganizationRepository orgaRepo;
        private IEventOrganizationRepository eventOrgaRepo;

        public EventRepository(CrmContext context, IEventContactRepository eventContactRepo, IEventOrganizationRepository eventOrgaRepo, IContactRepository contactRepo, IOrganizationRepository orgaRepo) : base(context)
        {
            this.contactRepo = contactRepo;
            this.eventOrgaRepo = eventOrgaRepo;
            this.orgaRepo = orgaRepo;
            this.eventContactRepo = eventContactRepo;
        }

        public async Task<Event> CreateNewEventAsync(EventCreateDto eventToCreate)
        {
            Event eventNew = new Event();
            eventNew.Date = eventToCreate.Date;
            eventNew.Duration = eventToCreate.Duration;
            eventNew.Name = eventToCreate.Name;
            eventNew.Time = eventToCreate.Time;
            eventNew.Description = eventToCreate.Description;
            eventNew.Location = eventToCreate.Location;
            eventToCreate.Contacts.ForEach(x => eventNew.Participated.Add(new Participated() { ObjectId = x, HasParticipated = false, WasInvited = false, ModelType = MODEL_TYPE.CONTACT }));
            eventToCreate.Organizations.ForEach(x => eventNew.Participated.Add(new Participated() { ObjectId = x, HasParticipated = false, WasInvited = false, ModelType = MODEL_TYPE.ORGANIZATION }));
            return await CreateAsync(eventNew);
        }

        public async Task<List<Event>> GetAllEventsWithAllIncludesAsync()
        {
            return await Entities
                .Include(a => a.Tags)
                .Include(b => b.Organizations)
                .ThenInclude(c => c.Organization)
                .Include(y => y.Contacts)
                .ThenInclude(z => z.Contact)
                .Include(x => x.Participated)
                .ToListAsync();
        }

        public async Task<Event> GetEventByIdWithAllIncludesAsync(long id)
        {
            return await Entities
                .Include(t => t.Tags)
                .Include(y => y.Contacts)
                .ThenInclude(z => z.Contact)
                .Include(a => a.Organizations)
                .ThenInclude(b => b.Organization)
                .Include(x => x.Participated)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Event>> GetEventsForContact(long contactId)
        {
            return await Entities
                .Include(y => y.Contacts)
                .ThenInclude(z => z.Contact)
                .Include(a => a.Participated)
                .Where(e => e.Contacts.Any(contact => contact.ContactId == contactId))
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsForOrganization(long orgaId)
        {
            return await Entities
                .Include(y => y.Organizations)
                .ThenInclude(z => z.Organization)
                .Include(a => a.Participated)
                .Where(e => e.Organizations.Any(orga => orga.OrganizationId == orgaId))
                .ToListAsync();
        }

        public async Task<bool> ModifyEventAsync(EventDto eventToModify)
        {
            Event eventExistent = await GetEventByIdWithAllIncludesAsync(eventToModify.Id);
            if (eventExistent != null)
            {
                List<EventContact> eventContacts = await eventContactRepo.GetAllAsync();
                eventContacts.RemoveAll(y => y.EventId != eventExistent.Id);
                List<EventOrganization> eventOrgas = await eventOrgaRepo.GetAllAsync();
                eventOrgas.RemoveAll(y => y.EventId != eventExistent.Id);
                eventExistent.Name = eventToModify.Name;
                eventExistent.Date = eventToModify.Date;
                eventExistent.Time = eventToModify.Time;
                eventExistent.Duration = eventToModify.Duration;
                eventExistent.Description = eventToModify.Description;
                eventExistent.Location = eventToModify.Location;
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
                    Participated participated = eventExistent.Participated.FirstOrDefault(x => x.ObjectId == part.ContactId && x.ModelType == MODEL_TYPE.CONTACT);
                    if (participated != null)
                    {
                        eventExistent.Participated.Remove(participated);
                    }
                    eventExistent.Contacts.Remove(part);
                    await RemoveEventContactAsync(part);
                }

                List<EventOrganization> eventOrgasToDelete = new List<EventOrganization>();
                eventOrgas.ForEach(x =>
                {
                    if (eventToModify.Organizations.FirstOrDefault(y => y.Id == x.OrganizationId) == null)
                    {
                        eventOrgasToDelete.Add(x);
                    }
                });
                foreach (EventOrganization part in eventOrgasToDelete)
                {
                    Participated participated = eventExistent.Participated.FirstOrDefault(x => x.ObjectId == part.OrganizationId && x.ModelType == MODEL_TYPE.ORGANIZATION);
                    if (participated != null)
                    {
                        eventExistent.Participated.Remove(participated);
                    }
                    eventExistent.Organizations.Remove(part);
                    await RemoveEventOrganizationAsync(part);
                }

                foreach (ParticipatedDto partNew in eventToModify.Participated)
                {
                    Participated participated = eventExistent.Participated.FirstOrDefault(y => y.Id == partNew.Id && partNew.Id > 0 && y.ModelType == partNew.ModelType);
                    if (participated != null)
                    {
                        participated.HasParticipated = partNew.HasParticipated;
                        participated.WasInvited = partNew.WasInvited;
                    }
                    else
                    {
                        eventExistent.Participated.Add(new Participated() { ObjectId = partNew.ObjectId, HasParticipated = partNew.HasParticipated, WasInvited = partNew.WasInvited, ModelType = partNew.ModelType });
                    }
                }

                foreach (ContactDto contact in eventToModify.Contacts)
                {
                    if (eventContacts.FirstOrDefault(y => y.ContactId == contact.Id) == null)
                    {
                        await AddEventContactAsync(new EventContact() { ContactId = contact.Id, EventId = eventExistent.Id });
                    }
                }

                foreach (OrganizationDto orga in eventToModify.Organizations)
                {
                    if (eventOrgas.FirstOrDefault(y => y.OrganizationId == orga.Id) == null)
                    {
                        await AddEventOrganizationAsync(new EventOrganization() { OrganizationId = orga.Id, EventId = eventExistent.Id });
                    }
                }

                List<Tag> tagsToAdd = new List<Tag>();
                List<Tag> tagsToRemove = new List<Tag>();
                foreach (TagDto tag in eventToModify.Tags)
                {
                    if (eventExistent.Tags.Find(a => a.Name.Equals(tag.Name)) == null)
                    {
                        tagsToAdd.Add(new Tag() { Id = 0, Name = tag.Name });
                    }
                }
                foreach (Tag tag in eventExistent.Tags)
                {
                    if (eventToModify.Tags.Find(a => a.Name.Equals(tag.Name)) == null)
                    {
                        tagsToRemove.Add(tag);
                    }
                }
                foreach (Tag tag in tagsToRemove)
                {
                    eventExistent.Tags.Remove(tag);
                }
                foreach (Tag tag in tagsToAdd)
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

        private async Task<bool> RemoveEventContactAsync(EventContact eventContact)
        {
            EventContact eventContactToDelete = await eventContactRepo.GetEventContactByEventContactAsync(eventContact);
            if (eventContactToDelete != null)
            {
                await eventContactRepo.DeleteAsync(eventContactToDelete);
                return true;
            }

            return false;
        }

        public async Task<EventOrganization> AddEventOrganizationAsync(EventOrganization eventOrganization)
        {
            Organization orga = await orgaRepo.GetByIdAsync(eventOrganization.OrganizationId);
            if (orga != null)
            {
                Event eventToInclude = await GetByIdAsync(eventOrganization.EventId);
                if (eventToInclude != null)
                {
                    EventOrganization result = await eventOrgaRepo.CreateAsync(new EventOrganization()
                    {
                        Organization = orga,
                        Event = eventToInclude,
                        OrganizationId = eventOrganization.OrganizationId,
                        EventId = eventOrganization.EventId
                    });
                    orga.Events.Add(result);
                    eventToInclude.Organizations.Add(result);
                    await orgaRepo.UpdateAsync(orga);
                    await UpdateAsync(eventToInclude);
                    return result;
                }
            }

            return null;
        }

        private async Task<bool> RemoveEventOrganizationAsync(EventOrganization eventOrga)
        {
            EventOrganization eventOrgaToDelete = await eventOrgaRepo.GetEventOrganizationByEventOrganizationAsync(eventOrga);
            if (eventOrgaToDelete != null)
            {
                await eventOrgaRepo.DeleteAsync(eventOrgaToDelete);
                return true;
            }

            return false;
        }
    }
}
