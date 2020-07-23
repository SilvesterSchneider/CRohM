using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using Microsoft.AspNetCore.Authorization;
using NSwag.Annotations;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModelLayer.Helper;

namespace WebApi.Controllers
{
    [Route("api/organization")]
    [ApiController]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService userService;
        private readonly IModificationEntryService modService;
        private readonly IOrganizationService _organizationService;
        private readonly ILogger _logger;
        private IContactService contactService;

        public OrganizationController(
            IMapper mapper,
            IOrganizationService organizationService,
            IUserService userService,
            ILoggerFactory logger,
            IModificationEntryService modService,
            IContactService contactService)
        {
            _mapper = mapper;
            this.contactService = contactService;
            this.userService = userService;
            this.modService = modService;
            _organizationService = organizationService;
            _logger = logger.CreateLogger(nameof(OrganizationController));
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<OrganizationDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var organizations = await _organizationService.GetAllOrganizationsWithIncludesAsync();
            var organizationsDto = _mapper.Map<List<OrganizationDto>>(organizations);

            return Ok(organizationsDto);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OrganizationDto), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> GetById(long id)
        {
            var organization = await _organizationService.GetByIdAsync(id);

            if (organization == null)
            {
                return NotFound();
            }

            var organizationDto = _mapper.Map<OrganizationDto>(organization);
            return Ok(organizationDto);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OrganizationDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "bad request")]
        public async Task<IActionResult> Put([FromBody]OrganizationDto organization, [FromRoute]long id, [FromQuery]long idOfUserChange)
        {
            if (organization == null)
            {
                return BadRequest();
            }
            if (id != organization.Id)
            {
                return BadRequest();
            }
            string userNameOfChange = await userService.GetUserNameByIdAsync(idOfUserChange);
            var mappedOrganization = _mapper.Map<Organization>(organization);
            await modService.UpdateOrganizationAsync(userNameOfChange, await _organizationService.GetByIdAsync(id), mappedOrganization, true);
            if (await _organizationService.UpdateAsyncWithAlleDependencies(mappedOrganization))
            {
                await modService.CommitChanges();
            }
            var organizationDto = _mapper.Map<OrganizationDto>(mappedOrganization);
            return Ok(organizationDto);
        }

        /// <summary>
        /// Add new contact to organization
        /// </summary>
        /// <param name="id">The id of the organization</param>
        /// <param name="contactId">The id of the contact which will be added</param>
        [HttpPut("{id}/addContact")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OrganizationDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "bad request")]
        public async Task<IActionResult> AddContact([FromRoute]long id, [FromBody]long contactId, [FromQuery]long idOfUserChange)
        {
            Organization organization;
            try
            {
                organization = await _organizationService.AddContactAsync(id, contactId);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest();
            }

            var organizationDto = _mapper.Map<OrganizationDto>(organization);
            string userNameOfChange = await userService.GetUserNameByIdAsync(idOfUserChange);
            string contactName = string.Empty;
            Contact contact = await contactService.GetByIdAsync(contactId);
            if (contact != null)
            {
                contactName = contact.PreName + " " + contact.Name;
            }
            await modService.ChangeEmployeesOfOrganization(id, contactName, false, userNameOfChange);
            return Ok(organizationDto);
        }

        /// <summary>
        /// Remove a contact from a organization
        /// </summary>
        /// <param name="id">The id of the organization</param>
        /// <param name="contactId">The id of the contact which will be removed</param>
        [HttpPut("{id}/removeContact")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "bad request")]
        public async Task<IActionResult> RemoveContact([FromRoute]long id, [FromBody]long contactId, [FromQuery]long idOfUserChange)
        {
            try
            {
                await _organizationService.RemoveContactAsync(id, contactId);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest();
            }
            string userNameOfChange = await userService.GetUserNameByIdAsync(idOfUserChange);
            string contactName = string.Empty;
            Contact contact = await contactService.GetByIdAsync(contactId);
            if (contact != null)
            {
                contactName = contact.PreName + " " + contact.Name;
            }
            await modService.ChangeEmployeesOfOrganization(id, contactName, true , userNameOfChange);
            return Ok();
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(OrganizationDto), Description = "successfully created")]
        public async Task<IActionResult> Post([FromBody]OrganizationCreateDto organizationToCreate, [FromQuery]long idOfUserChange)
        {
            Organization organization = await _organizationService.CreateAsync(_mapper.Map<Organization>(organizationToCreate));

            var organizationDto = _mapper.Map<OrganizationDto>(organization);
            var uri = $"https://{Request.Host}{Request.Path}/{organizationDto.Id}";
            string userNameOfChange = await userService.GetUserNameByIdAsync(idOfUserChange);
            await modService.CreateNewOrganizationEntryAsync(userNameOfChange, organization.Id);
            return Created(uri, organizationDto);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> Delete(long id)
        {
            Organization organization = await _organizationService.GetByIdAsync(id);
            if (organization == null)
            {
                return NotFound();
            }
            await _organizationService.DeleteAsync(organization);
            await modService.UpdateOrganizationByDeletionAsync(id);
            return Ok();
        }
    }
}
