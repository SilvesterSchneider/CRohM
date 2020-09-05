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

        public ContactController(IMapper mapper, IContactService contactService, IUserService userService, IEventService eventService, IModificationEntryService modService)
        {
            _mapper = mapper;
            this.eventService = eventService;
            this.userService = userService;
            this.modService = modService;
            this.contactService = contactService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ContactDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var contacts = await contactService.GetAllContactsWithAllIncludesAsync();
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts);

            return Ok(contactsDto);
        }

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

        [HttpGet("PartName")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ContactDto>), Description = "successfully found")]
        public async Task<IActionResult> Get([FromQuery]string name)
        {
            var contacts = await contactService.GetContactsByPartStringAsync(name);
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts);

            return Ok(contactsDto);
        }

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
            await modService.UpdateContactAsync(userOfModification, await contactService.GetByIdAsync(id), mappedContact, await userService.IsDatenschutzbeauftragter(userOfModification.Id));
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

        // sends disclosure per mail
        [HttpPost("{id}/disclosure")] // template ^= zusammen mit basis ganz oben -> pfad für http request
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully created")]
        public async Task<IActionResult> SendDisclosureById([FromRoute] long id)
        {
            await contactService.SendDisclosure(id);
            return Ok();
        }

        // deletes with id {id} contact via frontend
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
    }
}
