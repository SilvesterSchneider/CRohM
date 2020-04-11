using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IContactService contactService;

        public ContactController(IMapper mapper, IContactService contactService)
        {
            _mapper = mapper;
            this.contactService = contactService;
        }


        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ContactDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var contacts = await contactService.GetAllContactsWithAllIncludes();
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts);

            return Ok(contactsDto);
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
        public async Task<IActionResult> Put([FromBody]ContactDto contact, long id)
        {
            var mappedContact = _mapper.Map<Contact>(contact);
            await contactService.UpdateAsync(mappedContact);
            if (contact == null)
            {
                return Conflict();
            }
            var contactDto = _mapper.Map<ContactDto>(mappedContact);
            return Ok(contactDto);
        }

        // creates new contact in db via frontend
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(ContactDto), Description = "successfully created")]
        public async Task<IActionResult> Post([FromBody]ContactCreateDto contactToCreate)
        {

            Contact contact = await contactService.CreateAsync(_mapper.Map<Contact>(contactToCreate));

            var contactDto = _mapper.Map<ContactDto>(contact);
            var uri = $"https://{Request.Host}{Request.Path}/{contactDto.Id}";
            return Created(uri, contactDto);
        }

        // deletes with id {id} contact via frontend
        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> Delete(long id)
        {
            Contact contact = await contactService.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            await contactService.DeleteAsync(contact);
            return Ok();
        }
    }
}