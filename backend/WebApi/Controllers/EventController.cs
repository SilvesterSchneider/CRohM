using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;
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
        private IEventService eventService;

        public EventController(IMapper mapper, IEventService eventService)
        {
            this._mapper = mapper;
            this.eventService = eventService;
        }

        /// <summary>
        /// Getter für alle events als liste
        /// </summary>
        /// <returns></returns>
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
            await eventService.ModifyEventAsync(eventToModify);
            return Ok(eventToModify);
        }

        /// <summary>
        /// Ein kontakt einem event hinzufügen
        /// </summary>
        /// <param name="id">event id</param>
        /// <param name="contactId">kontakt id</param>
        /// <returns></returns>
        [HttpPut("{id}/addContact")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), Description = "bad request")]
        public async Task<IActionResult> AddContact([FromRoute]long id, [FromBody]long contactId)
        {
            EventContact result = await eventService.AddEventContactAsync(new EventContact() { EventId = id, ContactId = contactId });
            if (result != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Fahler beim hinzufügen des Kontakts zum event!");
            }
        }

        /// <summary>
        /// Einen kontakt aus einem event entfernen
        /// </summary>
        /// <param name="id">die event id</param>
        /// <param name="contactId">die kontakt id</param>
        /// <returns></returns>
        [HttpPut("{id}/removeContact")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), Description = "bad request")]
        public async Task<IActionResult> RemoveContact([FromRoute]long id, [FromBody]long contactId)
        {
            bool result = await eventService.RemoveEventContactAsync(new EventContact() { EventId = id, ContactId = contactId });
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Fehler beim löschen eines Kontakts aus einem Event!");
            }
        }

        /// <summary>
        /// Erzeugen eines neuen events
        /// </summary>
        /// <param name="eventToCreate">das zu erzeugende event</param>
        /// <returns></returns>
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
                var uri = $"https://{Request.Host}{Request.Path}/{_mapper.Map<EventDto>(newEvent).Id}";
                return Created(uri, eventToCreate);
            }
            return BadRequest("Fehler beim erzeugen eines Events!");
        }

        /// <summary>
        /// Löschen eines events anhand der id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
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
            return Ok();
        }
    }
}