using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using RepositoryLayer;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService userService;
        private readonly IModificationEntryService modService;
        private IEventService eventService;
        private IContactService contactService;
        private IOrganizationService orgaService;

        public EventController(IMapper mapper,
            IEventService eventService,
            IModificationEntryService modService,
            IUserService userService,
            IContactService contactService,
            IOrganizationService orgaService)
        {
            this._mapper = mapper;
            this.userService = userService;
            this.modService = modService;
            this.orgaService = orgaService;
            this.eventService = eventService;
            this.contactService = contactService;
        }

        /// <summary>
        /// Getter für alle events als liste
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Einsehen und Bearbeiten einer Veranstaltung")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<EventDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var events = await eventService.GetAllEventsWithAllIncludesAsync();
            var eventsDto = _mapper.Map<List<EventDto>>(events);

            return Ok(eventsDto);
        }

        /// <summary>
        /// Getter für event anhand id
        /// </summary>
        /// <param name="id">event id</param>
        /// <returns></returns>
        [Authorize(Roles = "Einsehen und Bearbeiten einer Veranstaltung")]
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EventDto), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> GetById(long id)
        {
            var eventToGet = await eventService.GetEventByIdWithAllIncludesAsync(id);

            if (eventToGet == null)
            {
                return NotFound();
            }

            var eventDto = _mapper.Map<EventDto>(eventToGet);
            return Ok(eventDto);
        }

        /// <summary>
        /// Einen bestehenden event aktualisieren
        /// </summary>
        /// <param name="eventToModify">das zu bearbeitende event</param>
        /// <param name="id">die event id</param>
        /// <returns></returns>
        [Authorize(Roles = "Einsehen und Bearbeiten einer Veranstaltung")]
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EventDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "bad request")]
        public async Task<IActionResult> Put([FromBody]EventDto eventToModify, [FromRoute]long id)
        {
            if (eventToModify == null)
            {
                return BadRequest();
            }
            if (id != eventToModify.Id)
            {
                return BadRequest();
            }
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            Event oldOne = await eventService.GetEventByIdWithAllIncludesAsync(id);
            List<Contact> contactsParticipated = new List<Contact>();
            List<Organization> orgasParticipated = new List<Organization>();
            foreach (ParticipatedDto part in eventToModify.Participated)
            {
                if (part.ModelType == MODEL_TYPE.CONTACT)
                {
                    Contact cont = await contactService.GetByIdAsync(part.ObjectId);
                    if (cont != null)
                    {
                        contactsParticipated.Add(cont);
                    }
                }
                else if (part.ModelType == MODEL_TYPE.ORGANIZATION)
                {
                    Organization orga = await orgaService.GetByIdAsync(part.ObjectId);
                    if (orga != null)
                    {
                        orgasParticipated.Add(orga);
                    }
                }
            }
            await modService.UpdateEventsAsync(userOfChange, oldOne, eventToModify, contactsParticipated, orgasParticipated);
            if (await eventService.ModifyEventAsync(eventToModify))
            {
                await modService.CommitChanges();
            }
            return Ok(eventToModify);
        }

        /// <summary>
        /// Erzeugen eines neuen events
        /// </summary>
        /// <param name="eventToCreate">das zu erzeugende event</param>
        /// <returns></returns>
        [Authorize(Roles = "Anlegen einer Veranstaltung")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(void), Description = "successfully created")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), Description = "bad request")]
        public async Task<IActionResult> Post([FromBody]EventCreateDto eventToCreate)
        {
            Event newEvent = await eventService.CreateNewEventAsync(eventToCreate);
            if (newEvent != null)
            {
                foreach (int contactId in eventToCreate.Contacts)
                {
                    await eventService.AddEventContactAsync(new EventContact() { ContactId = contactId, EventId = newEvent.Id });
                }
                foreach (int orgaId in eventToCreate.Organizations)
                {
                    await eventService.AddEventOrganizationAsync(new EventOrganization() { OrganizationId = orgaId, EventId = newEvent.Id });
                }
                var uri = $"https://{Request.Host}{Request.Path}/{_mapper.Map<EventDto>(newEvent).Id}";
                User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
                await modService.CreateNewEventEntryAsync(userOfChange, newEvent.Id);
                return Created(uri, eventToCreate);
            }
            return BadRequest("Fehler beim erzeugen eines Events!");
        }

        /// <summary>
        /// Löschen eines events anhand der id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [Authorize(Roles = "Löschen einer Veranstaltung")]
        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> Delete(long id)
        {
            Event eventToDelete = await eventService.GetEventByIdWithAllIncludesAsync(id);
            if (eventToDelete == null)
            {
                return NotFound();
            }
            await eventService.DeleteAsync(eventToDelete);
            await modService.UpdateEventByDeletionAsync(id);
            return Ok();
        }
    }
}
