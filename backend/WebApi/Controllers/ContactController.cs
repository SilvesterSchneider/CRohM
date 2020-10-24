// Copyright (C) Christ Electronic Systems GmbH

using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using RepositoryLayer;
using ServiceLayer;
using System.Linq;
using ModelLayer.Helper;
using WebApi.Helper;
using WebApi.Wrapper;
using System;

namespace WebApi.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEventService eventService;
        private IUserService userService;
        private readonly IModificationEntryService modService;
        private IContactService contactService;
        private IHistoryService historyService;

        public ContactController(IMapper mapper, IContactService contactService, IUserService userService, IEventService eventService, IModificationEntryService modService, IHistoryService  historyService)          
        {
            _mapper = mapper;
            this.eventService = eventService;
            this.userService = userService;
            this.modService = modService;
            this.contactService = contactService;
            this.historyService = historyService;
        }

        //[ClaimsAuthorize(ClaimType = "Einsehen und Bearbeiten aller Kontakte",
        //                    ClaimValue = "Einsehen und Bearbeiten aller Kontakte")]

        [Authorize(Policy = "Admin")] 
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ContactDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var contacts = await contactService.GetAllContactsWithAllIncludesAsync();
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts);

            return Ok(contactsDto);
        }
        //[ClaimsAuthorization(ClaimType = "Einsehen und Bearbeiten aller Kontakte",
        //                    ClaimValue = "Einsehen und Bearbeiten aller Kontakte")]
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ContactDto), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "contact not found")]
        public async Task<IActionResult> GetById(long id)
        {
            var contact = await contactService.GetByIdAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            var contactDto = _mapper.Map<ContactDto>(contact);
            return Ok(contactDto);
        }
        //[ClaimsAuthorization(ClaimType = "Einsehen und Bearbeiten aller Kontakte",
        //                    ClaimValue = "Einsehen und Bearbeiten aller Kontakte")]
        [HttpGet("PartName")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ContactDto>), Description = "successfully found")]
        public async Task<IActionResult> Get([FromQuery]string name)
        {
            var contacts = await contactService.GetContactsByPartStringAsync(name);
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts);

            return Ok(contactsDto);
        }

        //[ClaimsAuthorization(ClaimType = "Einsehen und Bearbeiten aller Kontakte",
        //                     ClaimValue = "Einsehen und Bearbeiten aller Kontakte")]
        // put updates contact with id {id} via frontend
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ContactDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> Put([FromRoute]long id, [FromBody]ContactDto contact)
        {
            if (contact == null)
            {
                return Conflict();
            }
            User userOfModification = await userService.FindByNameAsync(User.Identity.Name);

            var mappedContact = _mapper.Map<Contact>(contact);
            await modService.UpdateContactAsync(userOfModification, await contactService.GetByIdAsync(id), mappedContact, await userService.IsDataSecurityEngineer(userOfModification.Id));
            if (await contactService.UpdateAsync(mappedContact, id))
            {
                await modService.CommitChanges();
                return Ok(contact);
            }
            else
            {
                return Conflict();
            }
        }
        
        //[ClaimsAuthorization(ClaimType = "Anlegen eines Kontaktes", ClaimValue = "Anlegen eines Kontaktes")]
        // creates new contact in db via frontend
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(ContactDto), Description = "successfully created")]
        public async Task<IActionResult> Post([FromBody]ContactCreateDto contactToCreate)
        {
            Contact contact = await contactService.CreateAsync(_mapper.Map<Contact>(contactToCreate));

            var contactDto = _mapper.Map<ContactDto>(contact);
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            await modService.CreateNewContactEntryAsync(userOfChange, contact.Id);
            var uri = $"https://{Request.Host}{Request.Path}/{contactDto.Id}";
            return Created(uri, contactDto);
        }

        //[ClaimsAuthorization(ClaimType = "Hinzufügen eines Historieneintrags bei Kontakt oder Organisation",
        //                    ClaimValue = "Hinzufügen eines Historieneintrags bei Kontakt oder Organisation")]
        // creates new contact in db via frontend
        [HttpPost("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully created")]
        public async Task<IActionResult> PostHistoryElement([FromBody]HistoryElementCreateDto historyToCreate, [FromRoute]long id)
        {
            await contactService.AddHistoryElement(id, _mapper.Map<HistoryElement>(historyToCreate));
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            await modService.UpdateContactByHistoryElementAsync(userOfChange, id, historyToCreate.Name + ":" + historyToCreate.Comment);
            return Ok();
        }

        //[ClaimsAuthorization(ClaimType = "Auskunft gegenüber eines Kontakts zu dessen Daten",
        //                    ClaimValue = "Auskunft gegenüber eines Kontakts zu dessen Daten")]
        // sends disclosure per mail
        [HttpPost("{id}/disclosure")] // template ^= zusammen mit basis ganz oben -> pfad für http request
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully created")]
        public async Task<IActionResult> SendDisclosureById([FromRoute] long id)
        {
            await contactService.SendDisclosure(id);
            return Ok();
        }

        // deletes with id {id} contact via frontend
        [Authorize(Roles = "Löschen eines Kontakts")]
        //[ClaimsAuthorization(ClaimType = "Löschen eines Kontakts", ClaimValue = "Löschen eines Kontakts")]

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "contact not found")]
        public async Task<IActionResult> Delete(long id)
        {
            Contact contact = await contactService.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            await contactService.DeleteAsync(contact);
            await modService.UpdateContactByDeletionAsync(id);
            return Ok();
        }

        [HttpGet("{id}/history")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(PagedResponse<List<object>>), Description = "successfully found")]
        public async Task<IActionResult> GetHistory(long id, [FromQuery] PaginationFilter filter)
        {
            PaginationFilter validFilter = new PaginationFilter(filter.PageStart, filter.PageSize);

            List<Event> events = await eventService.GetEventsForContact(id);        
            List<HistoryElement> history = await historyService.GetHistoryByContactAsync(id);

            List<object> mergedResult = events.Cast<object>().Concat(history.Cast<object>()).ToList();
            mergedResult.Sort((e1, e2) => DateTime.Compare(getDate(e2), getDate(e1)));

            List<object> pagination = mergedResult.Skip(validFilter.PageStart).Take(validFilter.PageSize).ToList();
            return Ok(new PagedResponse<List<object>>(pagination, validFilter.PageStart, validFilter.PageSize, mergedResult.Count));
        }


        private DateTime getDate(object element)
        {
            if(element is Event)
            {
                return ((Event)element).Date;
            }

            if(element is HistoryElement)
            {
                return ((HistoryElement)element).Date;
            }

            return DateTime.Now;
        }
    }
}
