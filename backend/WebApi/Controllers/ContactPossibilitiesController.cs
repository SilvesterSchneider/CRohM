using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/contactPossibilities")]
    [ApiController]
    public class ContactPossibilitiesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IContactPossibilitiesService contactPossibilitiesService;

        public ContactPossibilitiesController(IMapper _mapper, IContactPossibilitiesService contactPossibilitiesService)
        {
            this._mapper = _mapper;
            this.contactPossibilitiesService = contactPossibilitiesService;
        }

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ContactPossibilitiesDto), Description = "successfully get")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "contact not found")]
        public async Task<IActionResult> GetContactPossibilitiesById(long id)
        {
            ContactPossibilities contact = await this.contactPossibilitiesService.GetContactPossibilityByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            ContactPossibilitiesDto contactDto = _mapper.Map<ContactPossibilitiesDto>(contact);
            return Ok(contactDto);
        }

        // put updates contact with id {id} via frontend
        [HttpPost("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully inserted")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in insertion process")]
        public async Task<IActionResult> AddNewEntry([FromBody]ContactPossibilitiesEntryCreateDto entry, long id)
        {
            ContactPossibilitiesEntry entryMapped = _mapper.Map<ContactPossibilitiesEntry>(entry);
            bool status = await this.contactPossibilitiesService.AddContactPossibilityEntryAsync(entryMapped, id);
            if (status)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

        // put updates contact with id {id} via frontend
        [HttpPut("{idEntry}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully removed")]
        public async Task<IActionResult> RemoveEntry([FromBody]long contactPossibilityId, long idEntry)
        {
            await this.contactPossibilitiesService.RemoveContactPossibilityEntryAsync(contactPossibilityId, idEntry);
            return Ok();
        }
    }
}
